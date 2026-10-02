<script setup lang="ts">
import type { PublicContext } from './types'

defineProps<{
  context: PublicContext
}>()
</script>

<template>
  <header class="ordering-hero">
    <img v-if="context.theme.logoUrl" :src="context.theme.logoUrl" alt="" class="ordering-logo">
    <span v-else class="ordering-unit-mark" aria-hidden="true">
      <q-icon name="storefront" />
    </span>
    <div class="ordering-context">
      <p class="ordering-unit-label">Cardápio da unidade</p>
      <h1>{{ context.establishmentName }}</h1>
      <p>Explore o cardápio e monte seu pedido.</p>
    </div>
    <span v-if="context.table" class="ordering-table-badge">
      <q-icon name="table_restaurant" aria-hidden="true" /> Mesa {{ context.table.code }}
    </span>
  </header>
</template>
