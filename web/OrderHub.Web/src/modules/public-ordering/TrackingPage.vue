<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import ProblemBanner from '../../components/ProblemBanner.vue'
import { publicOrderingClient } from './client'
import type { Tracking } from './types'
import { pollingDelay, terminalOrderStatuses } from './tracking'

const reference = String(useRoute().params.reference)
const order = ref<Tracking>()
const error = ref<unknown>(null)
const loading = ref(true)
const cancelling = ref(false)
const reason = ref('')
let timer: ReturnType<typeof setTimeout> | undefined
let failures = 0
const statusLabel: Record<string, string> = {
  Confirmed: 'Pedido confirmado', Preparing: 'Em preparação', Ready: 'Pronto',
  OutForDelivery: 'Saiu para entrega', Completed: 'Concluído',
  Cancelled: 'Cancelado', Rejected: 'Não aceito'
}
const canCancel = computed(() => order.value?.status === 'Confirmed')
const money = (value: number) => new Intl.NumberFormat('pt-BR', {
  style: 'currency', currency: 'BRL'
}).format(value)
function schedule() {
  clearTimeout(timer)
  if (order.value && terminalOrderStatuses.has(order.value.status)) return
  timer = setTimeout(refresh, pollingDelay(failures))
}
async function refresh() {
  if (document.visibilityState === 'hidden') { schedule(); return }
  try {
    order.value = await publicOrderingClient.track(reference)
    error.value = null
    failures = 0
  } catch (failure) {
    error.value = failure
    failures++
  } finally { loading.value = false; schedule() }
}
async function cancel() {
  if (!canCancel.value || cancelling.value) return
  cancelling.value = true
  error.value = null
  try {
    await publicOrderingClient.cancel(reference, reason.value.trim() || null)
    await refresh()
  } catch (failure) { error.value = failure } finally { cancelling.value = false }
}
function visible() { if (document.visibilityState === 'visible') void refresh() }
onMounted(() => { document.addEventListener('visibilitychange', visible); void refresh() })
onBeforeUnmount(() => { clearTimeout(timer); document.removeEventListener('visibilitychange', visible) })
</script>

<template>
  <q-page id="main-content" class="ordering-page">
    <main class="flow-panel tracking-panel">
      <h1>Acompanhe seu pedido</h1>
      <ProblemBanner :error="error">
        <q-btn flat label="Tentar novamente" @click="refresh" />
      </ProblemBanner>
      <div v-if="loading" role="status">Buscando pedido…</div>
      <template v-else-if="order">
        <p class="eyebrow">Pedido nº {{ order.number }}</p>
        <h2>{{ statusLabel[order.status] ?? order.status }}</h2>
        <p>Total <strong>{{ money(order.total) }}</strong></p>
        <ol class="timeline" aria-label="Histórico do pedido">
          <li v-for="event in order.history" :key="event.occurredAt + event.status">
            <strong>{{ statusLabel[event.status] ?? event.status }}</strong>
            <time :datetime="event.occurredAt">{{ new Date(event.occurredAt).toLocaleString('pt-BR') }}</time>
            <span v-if="event.note">{{ event.note }}</span>
          </li>
        </ol>
        <section v-if="canCancel" class="cancel-panel">
          <h3>Precisa cancelar?</h3>
          <q-input v-model="reason" label="Motivo (opcional)" />
          <q-btn outline color="negative" label="Cancelar pedido"
            :loading="cancelling" :disable="cancelling" @click="cancel" />
        </section>
        <p v-else-if="!terminalOrderStatuses.has(order.status)">
          O cancelamento pelo cliente não está mais disponível. Continue acompanhando por aqui.
        </p>
      </template>
    </main>
  </q-page>
</template>
