<script setup lang="ts">
import { computed } from 'vue'
import type { KitchenTicket } from './types'

const props = defineProps<{
  ticket: KitchenTicket
  now: number
  busyId: string | null
}>()

defineEmits<{ advance: [ticket: KitchenTicket] }>()

const elapsed = computed(() => {
  const start = props.ticket.preparationStartedAt ?? props.ticket.confirmedAt
  const minutes = Math.max(0, Math.floor((props.now - Date.parse(start)) / 60_000))
  return minutes < 1 ? 'agora' : `${minutes} min`
})
const isLate = computed(() => props.now - Date.parse(props.ticket.confirmedAt) >= 15 * 60_000)
const actionLabel = computed(() => props.ticket.action === 'StartPreparation'
  ? 'Iniciar preparo' : 'Marcar pronto')
const priorityLabel = computed(() => props.ticket.status === 'Preparing'
  ? 'Em preparo' : 'Aguardando preparo')
const serviceLabel = computed(() => {
  if (props.ticket.tableCode) return `Mesa ${props.ticket.tableCode}`
  return props.ticket.serviceType === 'Delivery' ? 'Entrega' : 'Retirada'
})
</script>

<template>
  <article class="kitchen-ticket" :class="{ late: isLate }" tabindex="0">
    <header>
      <div>
        <span class="ticket-number">#{{ ticket.number }}</span>
        <span class="service">{{ serviceLabel }}</span>
      </div>
      <div class="timer" :aria-label="`Tempo nesta etapa: ${elapsed}`">
        {{ elapsed }}
      </div>
    </header>
    <p class="priority-label" :class="ticket.status === 'Preparing' ? 'status-preparing' : 'status-confirmed'">{{ priorityLabel }}</p>
    <p v-if="isLate" class="late-label" role="status">Pedido atrasado</p>
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
        <p v-if="item.notes" class="item-note"><strong>Observação:</strong> {{ item.notes }}</p>
      </li>
    </ul>

    <q-btn
      class="ticket-action"
      color="primary"
      unelevated
      no-caps
      :label="actionLabel"
      :loading="busyId === ticket.id"
      :disable="busyId !== null && busyId !== ticket.id"
      @click="$emit('advance', ticket)"
    />
  </article>
</template>

<style scoped>
.kitchen-ticket { min-width: 0; margin-bottom: 14px; padding: 18px; overflow-wrap: anywhere; border: 2px solid transparent; border-radius: 12px; background: var(--oh-surface-raised); box-shadow: 0 2px 8px color-mix(in srgb, var(--oh-text-primary) 9%, transparent); }
.kitchen-ticket.late { border-color: var(--oh-status-urgency); }
.kitchen-ticket > header { display: flex; justify-content: space-between; align-items: flex-start; gap: 12px; }
.ticket-number { display: block; color: var(--oh-text-primary); font-size: 1.45rem; font-weight: 800; }
.service, .customer { color: var(--oh-text-muted); }
.timer { font-size: 1.05rem; font-weight: 800; white-space: nowrap; }
.priority-label { width: fit-content; margin: 10px 0; padding: 5px 9px; border-radius: 999px; font-weight: 700; }
.priority-label.status-confirmed { background: color-mix(in srgb, var(--oh-status-confirmed) 12%, white); color: var(--oh-status-confirmed); }
.priority-label.status-preparing { background: color-mix(in srgb, var(--oh-status-preparing) 22%, white); color: var(--oh-status-warning-text); }
.late-label { width: fit-content; margin: 10px 0; padding: 3px 8px; border-radius: 999px; background: color-mix(in srgb, var(--oh-status-urgency) 10%, var(--oh-surface-raised)); color: var(--oh-status-urgency); font-weight: 700; }
.ticket-items { margin: 16px 0; padding: 0; list-style: none; border-top: 1px solid var(--oh-border-subtle); }
.ticket-items > li { padding: 12px 0; border-bottom: 1px solid var(--oh-border-subtle); }
.item-name { margin: 0; font-size: 1.05rem; }
.additionals { margin-top: 6px; padding-left: 20px; color: var(--oh-text-muted); }
.item-note { margin: 8px 0 0; padding: 8px 10px; border-radius: 8px; background: color-mix(in srgb, var(--oh-status-warning) 12%, var(--oh-surface-raised)); font-weight: 600; }
.ticket-action { width: 100%; min-height: 52px; font-size: 1rem; }
</style>
