<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '../../session/store'
import { availableActions, serviceLabels, statusLabels, type OrderAction } from './actions'
import { PollingCoordinator } from './polling'
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
const serviceType = ref<OrderServiceType | undefined>(parseService(route.query.serviceType))
const search = ref<string | null>(typeof route.query.search === 'string' ? route.query.search : '')
const pendingAction = ref<OrderAction | null>(null)
const note = ref('')
const actionBusy = ref(false)
const detailPanel = ref<HTMLElement | null>(null)

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
      orders: store.orders.filter((order) => order.status === item)
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
const interval = Math.max(
  5_000,
  Number(import.meta.env.VITE_OPERATIONS_POLL_INTERVAL_MS) || 15_000
)
const poller = new PollingCoordinator(
  () => store.synchronize(session.unitId, filters.value),
  {
    intervalMs: interval,
    maxIntervalMs: Math.max(interval, 120_000),
    visibility: document
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

onMounted(() => poller.start())
onBeforeUnmount(() => {
  poller.stop()
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
        <h1 class="text-h4 q-my-none">Pedidos em andamento</h1>
        <p class="text-grey-7 q-mb-none">
          Atualização automática a cada {{ Math.round(interval / 1000) }} segundos
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

    <q-banner
      v-if="store.stale"
      class="bg-orange-1 text-brown-9 q-mb-md"
      role="alert"
    >
      <strong>⚠ Dados possivelmente desatualizados.</strong>
      {{ store.error }}
      <template #action>
        <q-btn flat no-caps label="Tentar novamente" @click="poller.refresh(true)" />
      </template>
    </q-banner>
    <p class="sync-status" aria-live="polite">
      <span v-if="store.lastSuccessAt">✓ Última sincronização: {{ time(store.lastSuccessAt) }}</span>
      <span v-else>Sincronização ainda não concluída.</span>
    </p>

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

    <q-banner v-if="!session.unitId" class="bg-blue-1 text-primary">
      Selecione uma unidade autorizada para acompanhar os pedidos.
    </q-banner>

    <div v-else class="operations-workspace">
      <section class="status-board" aria-label="Pedidos por estado" :aria-busy="store.loading">
        <article v-for="group in grouped" :key="group.status" class="status-column">
          <h2>
            {{ statusLabels[group.status] }}
            <q-badge :label="group.orders.length" color="grey-8" />
          </h2>
          <p v-if="!group.orders.length" class="empty-state">Nenhum pedido nesta etapa.</p>
          <button
            v-for="order in group.orders"
            :key="order.id"
            type="button"
            class="order-card"
            :class="{ selected: store.selectedId === order.id }"
            @click="openOrder(order)"
          >
            <span class="order-card-title">
              <strong>#{{ order.number }}</strong>
              <span>{{ elapsed(order.createdAt) }}</span>
            </span>
            <span>{{ serviceLabels[order.serviceType] }}</span>
            <span v-if="order.customerName">{{ order.customerName }}</span>
            <span>{{ money(order.total) }}</span>
            <span v-if="store.isNew(order.id)" class="signal new">✦ Novo</span>
            <span v-if="store.isLate(order)" class="signal late">⚠ Atrasado</span>
          </button>
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
          ⚠ {{ store.conflict }}
        </q-banner>
        <q-banner v-if="store.actionError" class="bg-red-1 text-negative q-my-md" role="alert">
          {{ store.actionError }}
        </q-banner>

        <dl class="detail-facts">
          <div><dt>Atendimento</dt><dd>{{ serviceLabels[selected.serviceType] }}</dd></div>
          <div v-if="selected.tableCode"><dt>Mesa</dt><dd>{{ selected.tableCode }}</dd></div>
          <div><dt>Pagamento</dt><dd>{{ selected.isFullyPaid ? '✓ Pago' : '◷ Pendente' }}</dd></div>
          <div><dt>Confirmado</dt><dd>{{ money(selected.confirmedAmount) }} de {{ money(selected.total) }}</dd></div>
        </dl>

        <h3>Itens</h3>
        <ul class="detail-items">
          <li v-for="item in selected.items" :key="item.id">
            <strong>{{ item.quantity }}× {{ item.productName }}</strong>
            <span v-if="item.variationName"> · {{ item.variationName }}</span>
            <small v-if="item.additionals.length">
              + {{ item.additionals.map((value) => value.name).join(', ') }}
            </small>
            <p v-if="item.notes" class="item-note">📝 {{ item.notes }}</p>
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
.operations-page { padding: 24px; background: #f6f7fb; min-height: calc(100vh - 72px); }
.operations-heading, .detail-heading, .order-card-title { display: flex; justify-content: space-between; align-items: center; gap: 16px; }
.operations-heading { margin-bottom: 16px; }
.sync-status { color: #475569; min-height: 24px; }
.operations-filters { display: grid; grid-template-columns: repeat(3, minmax(160px, 240px)); gap: 12px; margin-bottom: 20px; }
.operations-workspace { display: grid; grid-template-columns: minmax(0, 1fr) minmax(320px, 420px); gap: 20px; align-items: start; }
.status-board { display: grid; grid-template-columns: repeat(4, minmax(230px, 1fr)); gap: 16px; overflow-x: auto; padding-bottom: 12px; }
.status-column { min-height: 240px; padding: 12px; border-radius: 12px; background: #e9edf5; }
.status-column h2 { display: flex; justify-content: space-between; align-items: center; margin: 0 0 12px; font-size: 1rem; }
.empty-state { color: #64748b; font-size: .875rem; }
.order-card { display: grid; gap: 7px; width: 100%; margin-bottom: 10px; padding: 14px; border: 2px solid transparent; border-radius: 10px; background: white; color: #172033; text-align: left; cursor: pointer; box-shadow: 0 1px 3px #1720331a; }
.order-card:hover, .order-card.selected { border-color: var(--oh-color-primary); }
.signal { width: fit-content; padding: 2px 7px; border-radius: 999px; font-size: .75rem; font-weight: 700; }
.signal.new { background: #dbeafe; color: #1d4ed8; }
.signal.late { background: #ffedd5; color: #9a3412; }
.order-detail { position: sticky; top: 92px; max-height: calc(100vh - 116px); overflow: auto; padding: 20px; border-radius: 12px; background: white; box-shadow: 0 8px 30px #17203320; }
.detail-facts { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.detail-facts div { padding: 10px; border-radius: 8px; background: #f1f5f9; }
.detail-facts dt { color: #64748b; font-size: .75rem; }
.detail-facts dd { margin: 3px 0 0; font-weight: 700; }
.detail-items, .order-history { padding-left: 20px; }
.detail-items li, .order-history li { margin-bottom: 12px; }
.detail-items small, .order-history time, .order-history span { display: block; color: #64748b; }
.item-note { margin: 5px 0; padding: 7px; border-left: 3px solid #f59e0b; background: #fffbeb; }
.detail-actions { display: flex; flex-wrap: wrap; gap: 8px; position: sticky; bottom: -20px; margin: 20px -20px -20px; padding: 16px 20px; background: white; border-top: 1px solid #e2e8f0; }
.confirmation-card { width: min(92vw, 520px); }
@media (max-width: 1100px) {
  .operations-workspace { grid-template-columns: 1fr; }
  .status-board { grid-template-columns: repeat(4, minmax(260px, 1fr)); }
  .order-detail { position: static; max-height: none; }
}
@media (max-width: 700px) {
  .operations-page { padding: 16px; }
  .operations-heading { align-items: flex-start; flex-direction: column; }
  .operations-filters { grid-template-columns: 1fr; }
}
</style>
