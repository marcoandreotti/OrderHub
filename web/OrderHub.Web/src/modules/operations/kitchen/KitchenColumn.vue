<script setup lang="ts">
import KitchenTicketCard from './KitchenTicketCard.vue'
import type { KitchenTicket } from './types'

defineProps<{
  columnKey: string
  title: string
  tickets: KitchenTicket[]
  now: number
  busyId: string | null
}>()

defineEmits<{ advance: [ticket: KitchenTicket] }>()
</script>

<template>
  <section class="kds-column" :class="`kds-column--${columnKey}`" :aria-labelledby="`kds-${columnKey}`">
    <h2 :id="`kds-${columnKey}`">
      <span>{{ title }}</span>
      <q-badge color="grey-8" :label="tickets.length" />
    </h2>
    <p v-if="!tickets.length" class="empty-state">Nenhum pedido nesta etapa.</p>
    <KitchenTicketCard
      v-for="ticket in tickets"
      :key="ticket.id"
      :ticket="ticket"
      :now="now"
      :busy-id="busyId"
      @advance="$emit('advance', $event)"
    />
  </section>
</template>

<style scoped>
.kds-column { min-width: 0; min-height: 360px; padding: 16px; border-radius: var(--oh-border-radius); background: color-mix(in srgb, var(--oh-border-subtle) 48%, var(--oh-surface-page)); }
.kds-column h2 { position: relative; display: grid; place-items: center; min-height: 48px; margin: 0 0 14px; padding: 8px 44px; border-radius: 8px; font-size: 1.1rem; text-align: center; }
.kds-column--waiting h2 { background: color-mix(in srgb, var(--oh-status-confirmed) 14%, var(--oh-surface-raised)); color: var(--oh-status-confirmed); }
.kds-column--preparing h2 { background: color-mix(in srgb, var(--oh-status-preparing) 18%, var(--oh-surface-raised)); color: var(--oh-status-warning-text); }
.kds-column h2 :deep(.q-badge) { position: absolute; right: 10px; }
.empty-state { color: var(--oh-text-muted); }
@media (max-width: 600px) {
  .kds-column { min-height: 0; padding: 10px; }
}
</style>
