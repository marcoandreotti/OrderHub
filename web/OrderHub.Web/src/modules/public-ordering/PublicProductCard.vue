<script setup lang="ts">
import { computed } from 'vue'
import type { Product } from './types'

const props = defineProps<{
  product: Product
  formattedPrice: string
}>()

defineEmits<{ select: [product: Product] }>()

const image = computed(() => [...props.product.images].sort((a, b) => a.order - b.order)[0])
</script>

<template>
  <button
    class="product-card"
    type="button"
    :disabled="product.isAvailable === false"
    @click="$emit('select', product)"
  >
    <img v-if="image" :src="image.url" alt="">
    <span>
      <strong>{{ product.name }}</strong>
      <small v-if="product.description">{{ product.description }}</small>
      <b v-if="product.isAvailable !== false">A partir de {{ formattedPrice }}</b>
      <b v-else>
        Indisponível<span v-if="product.unavailabilityReason"> · {{ product.unavailabilityReason }}</span>
      </b>
    </span>
  </button>
</template>
