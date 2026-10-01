<script setup lang="ts">
import type { Product } from './client'

defineProps<{
  products: Product[]
  loading: boolean
  categoryName: (productId: string) => string
}>()

const emit = defineEmits<{ edit: [product: Product] }>()

const money = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL'
})

function principalImage(product: Product) {
  return (
    product.images.find((image) => image.isPrincipal)?.url ??
    [...product.images].sort((left, right) => left.order - right.order)[0]?.url
  )
}

function associations(product: Product) {
  const variations = product.variations.length
  const groups = product.additionalGroups.length
  return `${variations} ${variations === 1 ? 'variação' : 'variações'} · ${groups} ${groups === 1 ? 'grupo' : 'grupos'}`
}
</script>

<template>
  <div class="admin-product-grid" :aria-busy="loading">
    <article
      v-for="product in products"
      :key="product.id"
      class="admin-product-card"
      :class="{ 'admin-product-card--inactive': !product.isActive }"
    >
      <img
        v-if="principalImage(product)"
        :src="principalImage(product)"
        :alt="`Imagem de ${product.name}`"
        class="admin-product-card__image"
      />
      <div v-else class="admin-product-card__placeholder" aria-hidden="true">▧</div>
      <div class="admin-product-card__body">
        <div class="admin-product-card__heading">
          <div>
            <p class="admin-product-card__category">{{ categoryName(product.id) }}</p>
            <h2>{{ product.name }}</h2>
          </div>
          <span class="admin-product-card__state">
            {{ product.isActive ? 'Ativo' : 'Inativo' }}
          </span>
        </div>
        <strong>A partir de {{ money.format(product.basePrice) }}</strong>
        <p>{{ associations(product) }}</p>
        <q-btn
          square
          class="collection-action-btn"
          color="primary"
          icon="edit"
          :aria-label="`Editar ${product.name}`"
          :disable="loading"
          @click="emit('edit', product)"
        ><q-tooltip>Editar {{ product.name }}</q-tooltip></q-btn>
      </div>
    </article>
  </div>
</template>
