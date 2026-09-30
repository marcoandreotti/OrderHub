<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '../../session/store'
import { availableActions, orderStatusTone, serviceLabels, statusLabels, type OrderAction } from './actions'
import OperationsOrderCard from './OperationsOrderCard.vue'
import OperationsSyncStatus from './OperationsSyncStatus.vue'
import { isProductionDue, sortImmediateOrders, sortScheduledOrders } from './board'
import { PollingCoordinator } from './polling'
import {
  createOrderRealtimeConnection,
  OrderRealtimeCoordinator,
  type OrderRealtimeState
} from './realtime'
import { useOrderOperationsStore } from './store'
import type { OrderFilters, OrderServiceType, OrderStatus, OrderSummary } from './types'

const session = useSessionStore()
const store = useOrderOperationsStore()
const route = useRoute()
const router = useRouter()
const statusOptions = Object.entries(statusLabels).map(([value, label]) => ({ value, label }))
const serviceOptions = Object.entries(serviceLabels).map(([value, label]) => ({ value, label }))
const stateOrder: OrderStatus[] = ['Confirmed', 'Preparing', 'Ready', 'OutForDelivery', 'Completed', 'Cancelled', 'Rejected']
const status = ref<OrderStatus | undefined>(parseStatus(route.query.status))
const serviceType = ref<OrderServiceType | undefined>(parseService(route.query.serviceType) ?? (route.path.endsWith('/delivery') ? 'Delivery' : undefined))
const search = ref<string | null>(typeof route.query.search === 'string' ? route.query.search : '')
const pendingAction = ref<OrderAction | null>(null)
const note = ref('')
const actionBusy = ref(false)
const detailPanel = ref<HTMLElement | null>(null)
const realtimeState = ref<OrderRealtimeState>('disconnected')

const filters = computed<OrderFilters>(() => ({
  status: status.value,
  serviceType: serviceType.value,
  number: /^[1-9]\d*$/.test((search.value ?? '').trim()) ? Number(search.value) : undefined
}))
const grouped = computed(() =>
  stateOrder
    .filter((item) => !status.value || item === status.value)
    .map((item) => ({
      status: item,
      orders: store.orders.filter((order) => order.status === item),
      immediate: sortImmediateOrders(store.orders.filter((order) => order.status === item && !order.scheduledAtUtc)),
      scheduled: sortScheduledOrders(store.orders.filter((order) => order.status === item && !!order.scheduledAtUtc))
    }))
)
const selected = computed(() =>
  store.selectedId ? store.details[store.selectedId] : undefined
)
const selectedActions = computed(() =>
  selected.value
    ? availableActions(selected.value, session.context?.capabilities ?? [])
    : []
)
const cardAction = (order: OrderSummary) =>
  availableActions(order, session.context?.capabilities ?? [])[0]
const interval = Math.max(
  5_000,
  Number(import.meta.env.VITE_OPERATIONS_POLL_INTERVAL_MS) || 15_000
)
function promisedTime(order: Pick<OrderSummary, 'scheduledAtUtc' | 'scheduledTimeZoneId'>) {
  if (!order.scheduledAtUtc) return ''
  return new Intl.DateTimeFormat('pt-BR', {
    timeZone: order.scheduledTimeZoneId ?? undefined, dateStyle: 'short', timeStyle: 'short'
  }).format(new Date(order.scheduledAtUtc))
}
const poller = new PollingCoordinator(
  () => store.synchronize(session.unitId, filters.value),
  {
    intervalMs: interval,
    maxIntervalMs: Math.max(interval, 120_000),
    visibility: document
  }
)
const realtime = new OrderRealtimeCoordinator(
  createOrderRealtimeConnection(),
  poller,
  (state) => {
    realtimeState.value = state
  }
)

watch([status, serviceType, search], () => {
  const query: Record<string, string> = {}
  if (status.value) query.status = status.value
  if (serviceType.value) query.serviceType = serviceType.value
  if (search.value?.trim()) query.search = search.value.trim()
  void router.replace({ query })
  void poller.refresh(true)
})

watch(() => route.path, (path) => {
  if (path.endsWith('/delivery')) serviceType.value = 'Delivery'
  else if (serviceType.value === 'Delivery') serviceType.value = undefined
})

watch(
  () => session.unitId,
  (unit) => {
    store.reset(unit)
    void realtime.start(unit)
  }
)

onMounted(() => void realtime.start(session.unitId))
onBeforeUnmount(() => {
  void realtime.stop()
  store.reset()
})

async function openOrder(order: OrderSummary) {
  await store.select(session.unitId, order.id)
  await nextTick()
  detailPanel.value?.focus()
}

function requestAction(action: OrderAction) {
  if (action.destructive) {
    pendingAction.value = action
    note.value = ''
  } else void execute(action)
}

async function requestCardAction(order: OrderSummary, action: OrderAction) {
  await openOrder(order)
  requestAction(action)
}

function setConfirmation(open: boolean) {
  if (!open) pendingAction.value = null
}

async function execute(action = pendingAction.value) {
  if (!action || !selected.value) return
  actionBusy.value = true
  try {
    await store.transition(
      session.unitId,
      selected.value.id,
      action.transition,
      filters.value,
      note.value
    )
    pendingAction.value = null
  } catch {
    // O store preserva ProblemDetails e o estado autoritativo para a interface.
  } finally {
    actionBusy.value = false
    await nextTick()
    detailPanel.value?.focus()
  }
}

function parseStatus(value: unknown): OrderStatus | undefined {
  return typeof value === 'string' && value in statusLabels
    ? (value as OrderStatus)
    : undefined
}
function parseService(value: unknown): OrderServiceType | undefined {
  return typeof value === 'string' && value in serviceLabels
    ? (value as OrderServiceType)
    : undefined
}
function time(value: string | number) {
  return new Intl.DateTimeFormat('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
    day: typeof value === 'string' ? '2-digit' : undefined,
    month: typeof value === 'string' ? '2-digit' : undefined
  }).format(new Date(value))
}
function money(value: number) {
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value)
}
function elapsed(value: string) {
  const minutes = Math.max(0, Math.floor((Date.now() - Date.parse(value)) / 60_000))
  return minutes < 1 ? 'agora' : 'há ' + minutes + ' min'
}
</script>

<template>
  <q-page class="operations-page">
    <header class="operations-heading">
      <div>
        <p class="text-overline text-primary q-mb-xs">CENTRAL OPERACIONAL</p>
        <h1 class="text-h4 q-my-none">{{ route.path.endsWith('/delivery') ? 'Entregas em andamento' : 'Pedidos em andamento' }}</h1>
        <p class="text-grey-7 q-mb-none">
          Tempo real com reconciliação autoritativa e fallback automático
        </p>
      </div>
      <q-btn
        color="primary"
        outline
        no-caps
        label="Atualizar agora"
        :loading="store.loading"
        @click="poller.refresh(true)"
      />
    </header>

    <OperationsSyncStatus
      :has-unit="!!session.unitId"
      :realtime-state="realtimeState"
      :stale="store.stale"
      :error="store.error"
      :last-success-label="store.lastSuccessAt ? time(store.lastSuccessAt) : null"
      @retry="poller.refresh(true)"
    />

    <section class="operations-filters" aria-label="Filtros de pedidos">
      <q-select
        v-model="status"
        :options="statusOptions"
        emit-value
        map-options
        clearable
        outlined
        dense
        label="Estado"
      />
      <q-select
        v-model="serviceType"
        :options="serviceOptions"
        emit-value
        map-options
        clearable
        outlined
        dense
        label="Atendimento"
      />
      <q-input
        v-model="search"
        outlined
        dense
        clearable
        inputmode="numeric"
        label="Número do pedido"
      />
    </section>

    <q-banner v-if="!session.unitId" class="bg-blue-1 text-info">
      Selecione uma unidade autorizada para acompanhar os pedidos.
    </q-banner>

    <div v-else class="operations-workspace">
      <section class="status-board" aria-label="Pedidos por estado" :aria-busy="store.loading">
        <article v-for="group in grouped" :key="group.status" class="status-column">
          <h2 :class="`status-heading--${orderStatusTone[group.status]}`">
            {{ statusLabels[group.status] }}
            <q-badge :label="group.orders.length" color="grey-8" />
          </h2>
          <h3 v-if="group.immediate.length" class="queue-subheading">Imediatos</h3>
          <OperationsOrderCard
            v-for="order in group.immediate"
            :key="order.id"
            :order="order"
            :status-label="statusLabels[order.status]"
            :service-label="serviceLabels[order.serviceType]"
            :time-label="elapsed(order.createdAt)"
            :total-label="money(order.total)"
            :selected="store.selectedId === order.id"
            :is-new="store.isNew(order.id)"
            :is-late="store.isLate(order)"
            :production-due="isProductionDue(order)"
            late-label="Atrasado"
            :next-action="cardAction(order)"
            @select="openOrder"
            @action="requestCardAction"
          />
          <h3 v-if="group.scheduled.length" class="queue-subheading">Agendados</h3>
          <OperationsOrderCard
            v-for="order in group.scheduled"
            :key="order.id"
            :order="order"
            :status-label="statusLabels[order.status]"
            :service-label="serviceLabels[order.serviceType]"
            :time-label="promisedTime(order)"
            :total-label="money(order.total)"
            :selected="store.selectedId === order.id"
            :is-new="store.isNew(order.id)"
            :is-late="store.isLate(order)"
            :production-due="isProductionDue(order)"
            late-label="Horário ultrapassado"
            :next-action="cardAction(order)"
            @select="openOrder"
            @action="requestCardAction"
          />
          <p v-if="!group.orders.length" class="empty-state">Nenhum pedido nesta etapa.</p>
        </article>
      </section>

      <aside
        v-if="selected"
        ref="detailPanel"
        tabindex="-1"
        class="order-detail"
        aria-label="Detalhes do pedido"
      >
        <div class="detail-heading">
          <div>
            <p class="text-overline q-mb-xs">{{ statusLabels[selected.status] }}</p>
            <h2 class="q-my-none">Pedido #{{ selected.number }}</h2>
          </div>
          <q-btn flat round aria-label="Fechar detalhes" @click="store.selectedId = null">×</q-btn>
        </div>

        <q-banner v-if="store.conflict" class="bg-orange-1 text-brown-9 q-my-md" role="alert">
          {{ store.conflict }}
        </q-banner>
        <q-banner v-if="store.actionError" class="bg-red-1 text-negative q-my-md" role="alert">
          {{ store.actionError }}
        </q-banner>

        <dl class="detail-facts">
          <div v-if="selected.scheduledAtUtc"><dt>Agendado para</dt><dd>{{ promisedTime(selected) }} · {{ selected.scheduledTimeZoneId }}</dd></div>
          <div><dt>Atendimento</dt><dd>{{ serviceLabels[selected.serviceType] }}</dd></div>
          <div v-if="selected.tableCode"><dt>Mesa</dt><dd>{{ selected.tableCode }}</dd></div>
          <div><dt>Pagamento</dt><dd>{{ selected.isFullyPaid ? 'Pago' : 'Pendente' }}</dd></div>
          <div><dt>Confirmado</dt><dd>{{ money(selected.confirmedAmount) }} de {{ money(selected.total) }}</dd></div>
        </dl>
        <section v-if="selected.serviceType === 'Delivery' && selected.deliveryAddress" class="q-mb-md" aria-label="Endereço de entrega">
          <h3 class="text-subtitle1">Destino da entrega</h3>
          <address>{{ selected.deliveryAddress.street }}, {{ selected.deliveryAddress.number }}<span v-if="selected.deliveryAddress.complement"> · {{ selected.deliveryAddress.complement }}</span><br>
            {{ selected.deliveryAddress.neighborhood }} · {{ selected.deliveryAddress.city }}/{{ selected.deliveryAddress.state }} · CEP {{ selected.deliveryAddress.postalCode }}</address>
          <small v-if="selected.deliveryRegionName">{{ selected.deliveryRegionName }} · {{ money(selected.deliveryFee) }} · estimativa {{ selected.deliveryEstimatedMinutes }} min</small>
        </section>

        <h3>Itens</h3>
        <ul class="detail-items">
          <li v-for="item in selected.items" :key="item.id">
            <strong>{{ item.quantity }}× {{ item.productName }}</strong>
            <span v-if="item.variationName"> · {{ item.variationName }}</span>
            <small v-if="item.additionals.length">
              + {{ item.additionals.map((value) => value.name).join(', ') }}
            </small>
            <p v-if="item.notes" class="item-note"><strong>Observação:</strong> {{ item.notes }}</p>
          </li>
        </ul>

        <h3>Histórico</h3>
        <ol class="order-history">
          <li v-for="entry in selected.history" :key="entry.occurredAt + entry.newStatus">
            <strong>{{ statusLabels[entry.newStatus] }}</strong>
            <time :datetime="entry.occurredAt">{{ time(entry.occurredAt) }}</time>
            <span v-if="entry.note">{{ entry.note }}</span>
          </li>
        </ol>

        <div v-if="selectedActions.length" class="detail-actions" aria-label="Ações do pedido">
          <q-btn
            v-for="action in selectedActions"
            :key="action.transition"
            :color="action.destructive ? 'negative' : 'primary'"
            :outline="action.destructive"
            no-caps
            :label="action.label"
            :loading="actionBusy"
            @click="requestAction(action)"
          />
        </div>
      </aside>
    </div>

    <q-dialog
      :model-value="pendingAction !== null"
      @update:model-value="setConfirmation"
    >
      <q-card class="confirmation-card">
        <q-card-section>
          <h2 class="text-h6 q-my-none">Confirmar {{ pendingAction?.label.toLowerCase() }}</h2>
          <p>O servidor verificará novamente seu papel e o estado atual do pedido.</p>
          <q-input v-model="note" autofocus outlined type="textarea" label="Motivo ou observação (opcional)" />
        </q-card-section>
        <q-card-actions align="right">
          <q-btn v-close-popup flat no-caps label="Voltar" />
          <q-btn color="negative" no-caps label="Confirmar" :loading="actionBusy" @click="execute()" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped>
.operations-page { padding: 24px; background: var(--oh-surface-page); min-height: calc(100vh - 72px); }
.operations-heading, .detail-heading { display: flex; justify-content: space-between; align-items: center; gap: 16px; }
.operations-heading { margin-bottom: 16px; }
.operations-filters { display: grid; grid-template-columns: repeat(3, minmax(160px, 240px)); gap: 12px; margin-bottom: 20px; }
.operations-workspace { display: grid; grid-template-columns: minmax(0, 1fr) minmax(320px, 420px); gap: 20px; align-items: start; }
.status-board { display: grid; grid-template-columns: repeat(4, minmax(230px, 1fr)); gap: 16px; padding-bottom: 12px; }
.status-column { min-height: 240px; padding: 12px; border-radius: var(--oh-border-radius); background: color-mix(in srgb, var(--oh-border-subtle) 42%, var(--oh-surface-page)); }
.status-column h2 { display: flex; justify-content: space-between; align-items: center; margin: 0 0 12px; font-size: 1rem; }
.status-heading--confirmed { color: var(--oh-status-confirmed); }
.status-heading--preparing { color: var(--oh-status-warning-text); }
.status-heading--ready { color: var(--oh-status-success-text); }
.status-heading--success { color: var(--oh-status-success-text); }
.status-heading--info { color: var(--oh-status-info); }
.status-heading--danger { color: var(--oh-status-cancelled); }
.empty-state { color: var(--oh-text-muted); font-size: .875rem; }
.queue-subheading { margin: 12px 0 8px; font-size: .9rem; color: var(--oh-text-muted); }
.order-detail { position: sticky; top: 92px; max-height: calc(100vh - 116px); overflow: auto; padding: 20px; border-radius: var(--oh-border-radius); background: var(--oh-surface-raised); box-shadow: 0 8px 30px color-mix(in srgb, var(--oh-text-primary) 12%, transparent); }
.detail-facts { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.detail-facts div { padding: 10px; border-radius: 8px; background: var(--oh-surface-page); }
.detail-facts dt { color: var(--oh-text-muted); font-size: .75rem; }
.detail-facts dd { margin: 3px 0 0; font-weight: 700; }
.detail-items, .order-history { padding-left: 20px; }
.detail-items li, .order-history li { margin-bottom: 12px; }
.detail-items small, .order-history time, .order-history span { display: block; color: var(--oh-text-muted); }
.item-note { margin: 5px 0; padding: 7px; border-radius: 8px; background: color-mix(in srgb, var(--oh-status-warning) 12%, var(--oh-surface-raised)); }
.detail-actions { display: flex; flex-wrap: wrap; gap: 8px; position: sticky; bottom: -20px; margin: 20px -20px -20px; padding: 16px 20px; background: var(--oh-surface-raised); border-top: 1px solid var(--oh-border-subtle); }
.confirmation-card { width: min(92vw, 520px); }
@media (max-width: 1100px) {
  .operations-workspace { grid-template-columns: 1fr; }
  .status-board { grid-template-columns: repeat(2, minmax(0, 1fr)); }
  .order-detail { position: static; max-height: none; }
}
@media (max-width: 700px) {
  .operations-page { padding: 16px; }
  .operations-heading { align-items: flex-start; flex-direction: column; }
  .operations-filters { grid-template-columns: 1fr; }
  .status-board { grid-template-columns: 1fr; }
}
</style>
