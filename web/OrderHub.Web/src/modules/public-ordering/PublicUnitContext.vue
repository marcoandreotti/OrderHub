<script setup lang="ts">
import type { PublicContext } from './types'

defineProps<{
  context: PublicContext
}>()
</script>

<template>
  <header class="ordering-hero">
    <img v-if="context.theme.logoUrl" :src="context.theme.logoUrl" alt="" class="ordering-logo">
    <div class="ordering-context">
      <p class="eyebrow">Cardápio digital</p>
      <h1>{{ context.establishmentName }}</h1>
      <p v-if="context.table">Mesa {{ context.table.code }}</p>
    </div>
  </header>
</template>
