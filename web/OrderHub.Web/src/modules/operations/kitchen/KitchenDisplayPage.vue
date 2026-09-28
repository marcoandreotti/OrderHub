<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useSessionStore } from '../../session/store'
import { createOrderRealtimeConnection, OrderRealtimeCoordinator } from '../orders/realtime'
import { PollingCoordinator } from '../orders/polling'
import { useKitchenDisplayStore } from './store'
import type { KitchenTicket } from './types'

const session = useSessionStore()
const store = useKitchenDisplayStore()
const now = ref(Date.now())
const realtimeState = ref('disconnected')
let clock: ReturnType<typeof setInterval> | undefined

const poller = new PollingCoordinator(
  () => store.synchronize(session.unitId),
  { intervalMs: 10_000, maxIntervalMs: 60_000, visibility: document }
)
const realtime = new OrderRealtimeCoordinator(
  createOrderRealtimeConnection(),
  poller,
  (state) => { realtimeState.value = state }
)

const columns = computed(() => [
  { key: 'preparing', title: 'Em preparo', tickets: store.preparing },
  { key: 'waiting', title: 'Aguardando preparo', tickets: store.waiting }
])

watch(
  () => session.unitId,
  (unit) => {
    store.reset(unit)
    void realtime.start(unit)
  }
)

onMounted(() => {
  void realtime.start(session.unitId)
  clock = setInterval(() => { now.value = Date.now() }, 30_000)
})
onBeforeUnmount(() => {
  if (clock) clearInterval(clock)
  void realtime.stop()
  store.reset()
})

async function advance(ticket: KitchenTicket) {
  try {
    await store.execute(session.unitId, ticket)
  } catch {
    // O store mantém o ProblemDetails ou o snapshot reconciliado visível.
  }
}

function elapsed(ticket: KitchenTicket) {
  const start = ticket.preparationStartedAt ?? ticket.confirmedAt
  const minutes = Math.max(0, Math.floor((now.value - Date.parse(start)) / 60_000))
  return minutes < 1 ? 'agora' : `${minutes} min`
}
function isLate(ticket: KitchenTicket) {
  return now.value - Date.parse(ticket.confirmedAt) >= 15 * 60_000
}
function actionLabel(ticket: KitchenTicket) {
  return ticket.action === 'StartPreparation' ? 'Iniciar preparo' : 'Marcar pronto'
}
function serviceLabel(ticket: KitchenTicket) {
  if (ticket.tableCode) return `Mesa ${ticket.tableCode}`
  return ticket.serviceType === 'Delivery' ? 'Entrega' : 'Retirada'
}
function time(value: number) {
  return new Intl.DateTimeFormat('pt-BR', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value))
}
</script>

<template>
  <q-page class="kds-page">
    <header class="kds-heading">
      <div>
        <p class="text-overline text-primary q-mb-xs">COZINHA</p>
        <h1 class="text-h4 q-my-none">Fila de produção</h1>
        <p class="text-grey-7 q-mb-none">Prioridade por etapa e ordem de confirmação</p>
      </div>
      <q-btn
        outline
        color="primary"
        no-caps
        label="Atualizar agora"
        :loading="store.loading"
        @click="poller.refresh(true)"
      />
    </header>

    <q-banner v-if="store.stale" class="bg-orange-1 text-brown-9 q-mb-md" role="alert">
      <strong>⚠ Fila possivelmente desatualizada.</strong> {{ store.error }}
      <template #action>
        <q-btn flat no-caps label="Tentar novamente" @click="poller.refresh(true)" />
      </template>
    </q-banner>
    <q-banner v-if="store.conflict" class="bg-orange-1 text-brown-9 q-mb-md" role="alert">
      {{ store.conflict }}
    </q-banner>
    <q-banner v-if="store.actionError" class="bg-red-1 text-negative q-mb-md" role="alert">
      {{ store.actionError }}
    </q-banner>

    <p class="sync-status" aria-live="polite">
      <span v-if="realtimeState === 'connected'">● Tempo real conectado. </span>
      <span v-else>◷ Atualização periódica ativa. </span>
      <span v-if="store.lastSuccessAt">Última sincronização: {{ time(store.lastSuccessAt) }}</span>
    </p>

    <q-banner v-if="!session.unitId" class="bg-blue-1 text-primary">
      Selecione uma unidade autorizada para abrir a fila da cozinha.
    </q-banner>

    <section v-else class="kds-board" aria-label="Fila da cozinha" :aria-busy="store.loading">
      <section v-for="column in columns" :key="column.key" class="kds-column" :aria-labelledby="`kds-${column.key}`">
        <h2 :id="`kds-${column.key}`">
          {{ column.title }}
          <q-badge color="grey-8" :label="column.tickets.length" />
        </h2>
        <p v-if="!column.tickets.length" class="empty-state">Nenhum pedido nesta etapa.</p>

        <article
          v-for="ticket in column.tickets"
          :key="ticket.id"
          class="kitchen-ticket"
          :class="{ late: isLate(ticket) }"
        >
          <header>
            <div>
              <span class="ticket-number">#{{ ticket.number }}</span>
              <span class="service">{{ serviceLabel(ticket) }}</span>
            </div>
            <div class="timer" :aria-label="`Tempo nesta etapa: ${elapsed(ticket)}`">
              <span aria-hidden="true">◷</span> {{ elapsed(ticket) }}
            </div>
          </header>
          <p v-if="isLate(ticket)" class="late-label" role="status">⚠ Pedido atrasado</p>
          <p v-if="ticket.customerName" class="customer">{{ ticket.customerName }}</p>

          <ul class="ticket-items">
            <li v-for="item in ticket.items" :key="item.id">
              <p class="item-name">
                <strong>{{ item.quantity }}× {{ item.productName }}</strong>
                <span v-if="item.variationName"> — {{ item.variationName }}</span>
              </p>
              <ul v-if="item.additionals.length" class="additionals" aria-label="Modificadores">
                <li v-for="additional in item.additionals" :key="additional.name">
                  + {{ additional.quantity }}× {{ additional.name }}
                </li>
              </ul>
              <p v-if="item.notes" class="item-note">📝 {{ item.notes }}</p>
            </li>
          </ul>

          <q-btn
            class="ticket-action"
            color="primary"
            unelevated
            no-caps
            :label="actionLabel(ticket)"
            :loading="store.busyId === ticket.id"
            :disable="store.busyId !== null && store.busyId !== ticket.id"
            @click="advance(ticket)"
          />
        </article>
      </section>
    </section>
  </q-page>
</template>

<style scoped>
.kds-page { min-height: calc(100vh - 72px); padding: 24px; background: #f4f6fa; }
.kds-heading { display: flex; justify-content: space-between; align-items: center; gap: 16px; margin-bottom: 12px; }
.sync-status { min-height: 24px; color: #475569; }
.kds-board { display: grid; grid-template-columns: repeat(2, minmax(320px, 1fr)); gap: 20px; align-items: start; }
.kds-column { min-height: 360px; padding: 16px; border-radius: 14px; background: #e5e9f1; }
.kds-column h2 { display: flex; justify-content: space-between; align-items: center; margin: 0 0 14px; font-size: 1.1rem; }
.empty-state { color: #64748b; }
.kitchen-ticket { margin-bottom: 14px; padding: 18px; border: 2px solid transparent; border-radius: 12px; background: white; box-shadow: 0 2px 8px #17203318; }
.kitchen-ticket.late { border-color: #ea580c; }
.kitchen-ticket > header { display: flex; justify-content: space-between; align-items: flex-start; gap: 12px; }
.ticket-number { display: block; color: #172033; font-size: 1.45rem; font-weight: 800; }
.service, .customer { color: #475569; }
.timer { font-size: 1.05rem; font-weight: 800; white-space: nowrap; }
.late-label { width: fit-content; margin: 10px 0; padding: 3px 8px; border-radius: 999px; background: #ffedd5; color: #9a3412; font-weight: 700; }
.ticket-items { margin: 16px 0; padding: 0; list-style: none; border-top: 1px solid #e2e8f0; }
.ticket-items > li { padding: 12px 0; border-bottom: 1px solid #e2e8f0; }
.item-name { margin: 0; font-size: 1.05rem; }
.additionals { margin-top: 6px; padding-left: 20px; color: #475569; }
.item-note { margin: 8px 0 0; padding: 8px 10px; border-left: 4px solid #f59e0b; background: #fffbeb; font-weight: 600; }
.ticket-action { width: 100%; min-height: 48px; font-size: 1rem; }
@media (max-width: 900px) {
  .kds-board { grid-template-columns: 1fr; }
}
@media (max-width: 600px) {
  .kds-page { padding: 14px; }
  .kds-heading { align-items: flex-start; flex-direction: column; }
  .kds-column { min-height: 0; padding: 10px; }
}
</style>
