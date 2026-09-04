<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ProblemBanner from '../../components/ProblemBanner.vue'
import { publicOrderingClient } from './client'
import { hydrateCartFromCatalog, loadCart, orderItems, receiptStorage, usePublicCart } from './cart'
import { applyPublicTheme } from './theme'
import { checkoutValidation } from './checkout'
import type {
  Address, Confirmation, Product, PublicCatalog, PublicContext,
  ServiceType, Simulation
} from './types'

type Step = 'catalog' | 'cart' | 'checkout' | 'receipt'
const route = useRoute()
const router = useRouter()
const slug = computed(() => String(route.params.slug))
const tableToken = computed(() => route.params.tableToken ? String(route.params.tableToken) : undefined)
const context = ref<PublicContext>()
const catalog = ref<PublicCatalog>()
const loading = ref(true)
const error = ref<unknown>(null)
const step = ref<Step>('catalog')
const selected = ref<Product>()
const quantity = ref(1)
const variationId = ref<string | null>(null)
const notes = ref('')
const selections = reactive<Record<string, string[]>>({})
const compositionError = ref('')
const cart = usePublicCart()
const simulation = ref<Simulation>()
const simulating = ref(false)
const priceChanged = ref(false)
const submitting = ref(false)
const confirmation = ref<Confirmation>()
const previousReference = ref<string | null>(null)
const idempotencyKey = ref<string>()
let controller: AbortController | undefined

const customer = reactive({ name: '', phone: '', email: '' })
const address = reactive<Address>({
  label: 'Principal', street: '', number: '', complement: null,
  neighborhood: '', city: '', state: '', postalCode: ''
})
const checkout = reactive({
  serviceType: 'Pickup' as ServiceType,
  couponCode: '',
  paymentMethodId: '',
  receivedAmount: null as number | null
})
const money = (value: number) => new Intl.NumberFormat('pt-BR', {
  style: 'currency', currency: 'BRL'
}).format(value)
const sortedCategories = computed(() => [...(catalog.value?.categories ?? [])]
  .filter(category => category.isActive)
  .sort((a, b) => a.order - b.order))
const serviceOptions = computed(() => context.value?.table
  ? [{ label: 'Nesta mesa', value: 'Table' }, { label: 'Retirada', value: 'Pickup' }, { label: 'Entrega', value: 'Delivery' }]
  : [{ label: 'Retirada', value: 'Pickup' }, { label: 'Entrega', value: 'Delivery' }])
const activeMethods = computed(() => context.value?.paymentMethods ?? [])
const canCheckout = computed(() => cart.state.items.length > 0)

async function load() {
  controller?.abort()
  controller = new AbortController()
  loading.value = true
  error.value = null
  loadCart(slug.value)
  previousReference.value = receiptStorage.read(slug.value)
  try {
    const [resolvedContext, resolvedCatalog] = await Promise.all([
      publicOrderingClient.context(slug.value, tableToken.value, controller.signal),
      publicOrderingClient.catalog(slug.value, controller.signal)
    ])
    context.value = resolvedContext
    catalog.value = resolvedCatalog
    hydrateCartFromCatalog(resolvedCatalog)
    checkout.serviceType = resolvedContext.table ? 'Table' : 'Pickup'
    checkout.paymentMethodId = resolvedContext.paymentMethods[0]?.id ?? ''
    applyPublicTheme(resolvedContext)
  } catch (failure) { error.value = failure } finally { loading.value = false }
}
function openProduct(product: Product) {
  selected.value = product
  quantity.value = 1
  variationId.value = product.variations.filter(x => x.isActive).sort((a, b) => a.order - b.order)[0]?.id ?? null
  notes.value = ''
  compositionError.value = ''
  Object.keys(selections).forEach(key => delete selections[key])
  product.additionalGroups.filter(x => x.isActive).forEach(group => { selections[group.id] = [] })
}
function toggle(groupId: string, additionalId: string, maximum: number) {
  const values = selections[groupId] ?? []
  const index = values.indexOf(additionalId)
  if (index >= 0) values.splice(index, 1)
  else if (values.length < maximum) values.push(additionalId)
}
function addProduct() {
  const product = selected.value
  if (!product) return
  const invalid = product.additionalGroups.filter(group => group.isActive).find(group => {
    const count = selections[group.id]?.length ?? 0
    return count < group.minimumSelection || count > group.maximumSelection
  })
  if (invalid) {
    compositionError.value = invalid.name + ': selecione entre ' +
      invalid.minimumSelection + ' e ' + invalid.maximumSelection + '.'
    return
  }
  const variation = product.variations.find(item => item.id === variationId.value)
  const additionals = product.additionalGroups.flatMap(group =>
    group.items.filter(item => selections[group.id]?.includes(item.id))
  )
  cart.add({
    key: crypto.randomUUID(), productId: product.id, variationId: variation?.id ?? null,
    productName: product.name, variationName: variation?.name ?? null,
    displayedUnitPrice: (variation?.price ?? product.basePrice) +
      additionals.reduce((sum, item) => sum + item.price, 0),
    quantity: quantity.value, notes: notes.value.trim() || null,
    additionals: additionals.map(item => ({ additionalId: item.id, quantity: 1 }))
  })
  selected.value = undefined
}
function request(deliveryAddress: Address | null = null) {
  return {
    serviceType: checkout.serviceType,
    customerId: null,
    customerAddressId: null,
    tableToken: checkout.serviceType === 'Table' ? context.value?.table?.token ?? null : null,
    deliveryAddress,
    couponCode: checkout.couponCode.trim() || null,
    paymentMethodId: checkout.paymentMethodId || null,
    items: orderItems(cart.state.items)
  }
}
async function simulate() {
  if (!canCheckout.value) return
  simulating.value = true
  error.value = null
  try {
    const result = await publicOrderingClient.simulate(
      slug.value,
      request(checkout.serviceType === 'Delivery' ? address : null)
    )
    priceChanged.value = simulation.value
      ? simulation.value.total !== result.total || simulation.value.subtotal !== result.subtotal
      : result.subtotal !== cart.displayedTotal.value
    simulation.value = result
  } catch (failure) { error.value = failure; simulation.value = undefined }
  finally { simulating.value = false }
}
function validateCheckout() {
  return checkoutValidation(
    checkout.serviceType, checkout.paymentMethodId, customer, address
  )
}
async function confirm() {
  if (submitting.value) return
  const validation = validateCheckout()
  if (validation) { error.value = new Error(validation); return }
  submitting.value = true
  error.value = null
  try {
    let customerId: string | null = null
    let customerAddressId: string | null = null
    if (checkout.serviceType !== 'Table') {
      const identified = await publicOrderingClient.customer(slug.value, {
        name: customer.name.trim(), phone: customer.phone.trim(),
        email: customer.email.trim() || null,
        address: checkout.serviceType === 'Delivery' ? address : null
      })
      customerId = identified.customerId
      customerAddressId = checkout.serviceType === 'Delivery' ? identified.addressId : null
    }
    const finalSimulation = await publicOrderingClient.simulate(slug.value, {
      ...request(null), customerId, customerAddressId
    })
    if (simulation.value && simulation.value.total !== finalSimulation.total) {
      simulation.value = finalSimulation
      priceChanged.value = true
      error.value = new Error('O total mudou. Confira os valores atualizados e confirme novamente.')
      return
    }
    simulation.value = finalSimulation
    idempotencyKey.value ??= crypto.randomUUID()
    const result = await publicOrderingClient.confirm(slug.value, {
      ...request(null), customerId, customerAddressId,
      paymentMethodId: checkout.paymentMethodId,
      receivedAmount: checkout.receivedAmount
    }, idempotencyKey.value)
    confirmation.value = result
    receiptStorage.save(slug.value, result.reference)
    cart.clear()
    step.value = 'receipt'
    idempotencyKey.value = undefined
  } catch (failure) { error.value = failure } finally { submitting.value = false }
}
function editIntent() {
  idempotencyKey.value = undefined
  priceChanged.value = false
}
watch(() => [cart.state.revision, checkout.serviceType, checkout.couponCode,
  checkout.paymentMethodId, checkout.receivedAmount, customer.name, customer.phone,
  customer.email, ...Object.values(address)], editIntent)
watch(() => [slug.value, tableToken.value], load)
onMounted(load)
onBeforeUnmount(() => controller?.abort())
</script>

<template>
  <q-page id="main-content" class="ordering-page">
    <div v-if="loading" class="state-panel" role="status" aria-live="polite">
      <q-spinner size="42px" /><p>Carregando cardápio…</p>
    </div>
    <div v-else-if="!context || !catalog" class="state-panel">
      <ProblemBanner :error="error" />
      <h1>Pedidos indisponíveis</h1>
      <p>Esta unidade não está aceitando pedidos neste endereço.</p>
      <q-btn label="Tentar novamente" color="primary" @click="load" />
    </div>
    <template v-else>
      <header class="ordering-hero">
        <img v-if="context.theme.logoUrl" :src="context.theme.logoUrl" alt="" class="ordering-logo">
        <div><p class="eyebrow">Cardápio digital</p><h1>{{ context.establishmentName }}</h1>
          <p v-if="context.table">Mesa {{ context.table.code }}</p>
        </div>
        <q-btn v-if="step === 'catalog'" :label="'Carrinho (' + cart.count.value + ')'"
          color="primary" :disable="!canCheckout" @click="step = 'cart'; simulate()" />
      </header>
      <aside v-if="step === 'catalog' && previousReference" class="resume-order">
        <span>Você tem um pedido recente.</span>
        <q-btn flat label="Retomar acompanhamento"
          @click="router.push('/order/track/' + previousReference)" />
      </aside>
      <ProblemBanner :error="error">
        <q-btn v-if="step !== 'catalog'" flat label="Recalcular" @click="simulate" />
      </ProblemBanner>

      <main v-if="step === 'catalog'" aria-label="Cardápio">
        <section v-for="category in sortedCategories" :key="category.id" class="category">
          <h2>{{ category.name }}</h2><p v-if="category.description">{{ category.description }}</p>
          <div class="product-grid">
            <button v-for="product in category.products.filter(x => x.isActive)" :key="product.id"
              class="product-card" type="button" @click="openProduct(product)">
              <img v-if="product.images[0]" :src="product.images.slice().sort((a,b) => a.order-b.order)[0]!.url" alt="">
              <span><strong>{{ product.name }}</strong><small>{{ product.description }}</small>
                <b>A partir de {{ money(product.basePrice) }}</b></span>
            </button>
          </div>
        </section>
        <p v-if="!sortedCategories.length" class="state-panel">Nenhum item disponível no momento.</p>
      </main>

      <main v-else-if="step === 'cart'" class="flow-panel">
        <h2>Seu carrinho</h2>
        <div v-if="!cart.state.items.length" class="state-panel"><p>Seu carrinho está vazio.</p></div>
        <ul v-else class="cart-list">
          <li v-for="item in cart.state.items" :key="item.key">
            <span><strong>{{ item.quantity }}× {{ item.productName }}</strong>
              <small v-if="item.variationName">{{ item.variationName }}</small></span>
            <span>{{ money(item.displayedUnitPrice * item.quantity) }}
              <button type="button" class="text-button" :aria-label="'Remover ' + item.productName"
                @click="cart.remove(item.key); simulate()">Remover</button></span>
          </li>
        </ul>
        <div v-if="simulating" role="status">Recalculando totais…</div>
        <div v-else-if="simulation" class="totals" aria-live="polite">
          <span>Subtotal <b>{{ money(simulation.subtotal) }}</b></span>
          <span>Desconto <b>− {{ money(simulation.discount) }}</b></span>
          <span>Taxas <b>{{ money(simulation.fees) }}</b></span>
          <span class="grand-total">Total atualizado <b>{{ money(simulation.total) }}</b></span>
          <p v-if="priceChanged" role="alert">O total mudou. Confira os valores atualizados antes de confirmar.</p>
        </div>
        <div class="flow-actions"><q-btn flat label="Voltar ao cardápio" @click="step = 'catalog'" />
          <q-btn label="Continuar" color="primary" :disable="!simulation || !canCheckout"
            @click="step = 'checkout'" /></div>
      </main>

      <main v-else-if="step === 'checkout'" class="flow-panel">
        <h2>Finalizar pedido</h2>
        <q-form @submit="confirm">
          <fieldset><legend>Como você quer receber?</legend>
            <q-option-group v-model="checkout.serviceType" :options="serviceOptions" type="radio" />
          </fieldset>
          <fieldset v-if="checkout.serviceType !== 'Table'"><legend>Seus dados</legend>
            <q-input v-model="customer.name" label="Nome" autocomplete="name" />
            <q-input v-model="customer.phone" label="Telefone" autocomplete="tel" />
            <q-input v-model="customer.email" label="E-mail (opcional)" autocomplete="email" type="email" />
          </fieldset>
          <fieldset v-if="checkout.serviceType === 'Delivery'"><legend>Endereço de entrega</legend>
            <q-input v-model="address.postalCode" label="CEP" autocomplete="postal-code" />
            <q-input v-model="address.street" label="Rua" autocomplete="address-line1" />
            <q-input v-model="address.number" label="Número" />
            <q-input v-model="address.complement" label="Complemento (opcional)" />
            <q-input v-model="address.neighborhood" label="Bairro" />
            <q-input v-model="address.city" label="Cidade" />
            <q-input v-model="address.state" label="Estado" maxlength="2" />
          </fieldset>
          <fieldset><legend>Pagamento e desconto</legend>
            <q-select v-model="checkout.paymentMethodId" label="Forma de pagamento"
              emit-value map-options :options="activeMethods.map(x => ({ label: x.name, value: x.id }))" />
            <q-input v-model="checkout.couponCode" label="Cupom (opcional)" />
            <q-input v-if="activeMethods.find(x => x.id === checkout.paymentMethodId)?.allowsChange"
              v-model.number="checkout.receivedAmount" label="Troco para" type="number" min="0" />
          </fieldset>
          <div v-if="simulation" class="grand-total">Total {{ money(simulation.total) }}</div>
          <div class="flow-actions"><q-btn flat label="Voltar ao carrinho" @click="step = 'cart'" />
            <q-btn type="submit" label="Confirmar pedido" color="primary"
              :loading="submitting" :disable="submitting" /></div>
        </q-form>
      </main>

      <main v-else class="flow-panel receipt" aria-live="polite">
        <p class="receipt-mark" aria-hidden="true">✓</p><h2>Pedido confirmado</h2>
        <p>Pedido nº <strong>{{ confirmation?.number }}</strong></p>
        <p>Total {{ money(confirmation?.total ?? 0) }}</p>
        <p class="public-reference">Referência: {{ confirmation?.reference }}</p>
        <q-btn label="Acompanhar pedido" color="primary"
          @click="router.push('/order/track/' + confirmation?.reference)" />
      </main>
    </template>

    <q-dialog :model-value="!!selected" @update:model-value="value => { if (!value) selected = undefined }">
      <q-card v-if="selected" class="composition-card">
        <q-card-section><h2>{{ selected.name }}</h2><p>{{ selected.description }}</p></q-card-section>
        <q-card-section>
          <fieldset v-if="selected.variations.filter(x => x.isActive).length"><legend>Escolha uma opção</legend>
            <label v-for="variation in selected.variations.filter(x => x.isActive).sort((a,b) => a.order-b.order)" :key="variation.id">
              <input v-model="variationId" type="radio" :value="variation.id"> {{ variation.name }} — {{ money(variation.price) }}
            </label>
          </fieldset>
          <fieldset v-for="group in selected.additionalGroups.filter(x => x.isActive).sort((a,b) => a.order-b.order)" :key="group.id">
            <legend>{{ group.name }} ({{ group.minimumSelection }}–{{ group.maximumSelection }})</legend>
            <label v-for="item in group.items.filter(x => x.isActive).sort((a,b) => a.order-b.order)" :key="item.id">
              <input type="checkbox" :checked="selections[group.id]?.includes(item.id)"
                :disabled="!selections[group.id]?.includes(item.id) && (selections[group.id]?.length ?? 0) >= group.maximumSelection"
                @change="toggle(group.id, item.id, group.maximumSelection)">
              {{ item.name }} <span>+ {{ money(item.price) }}</span>
            </label>
          </fieldset>
          <q-input v-if="selected.allowsNotes" v-model="notes" label="Observações" type="textarea" />
          <q-input v-model.number="quantity" label="Quantidade" type="number" min="1" />
          <p v-if="compositionError" role="alert" class="text-negative">{{ compositionError }}</p>
        </q-card-section>
        <q-card-actions align="right"><q-btn flat label="Cancelar" @click="selected = undefined" />
          <q-btn color="primary" label="Adicionar" @click="addProduct" /></q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
