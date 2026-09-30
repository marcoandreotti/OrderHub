<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ProblemBanner from '../../components/ProblemBanner.vue'
import { ApiError } from '../../http/client'
import PublicCatalogSearch from './PublicCatalogSearch.vue'
import PublicCartAccess from './PublicCartAccess.vue'
import PublicCategoryNavigation from './PublicCategoryNavigation.vue'
import PublicProductCard from './PublicProductCard.vue'
import PublicUnitContext from './PublicUnitContext.vue'
import { publicOrderingClient } from './client'
import { hydrateCartFromCatalog, loadCart, orderItems, receiptStorage, usePublicCart } from './cart'
import { applyPublicTheme } from './theme'
import { checkoutValidation } from './checkout'
import type {
  Address, Confirmation, Product, PublicCatalog, PublicContext,
  SchedulingSlots, ServiceType, Simulation
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
const catalogSearch = ref('')
const activeCategoryId = ref<string | null>(null)
const selected = ref<Product>()
const quantity = ref(1)
const variationId = ref<string | null>(null)
const notes = ref('')
const selections = reactive<Record<string, string[]>>({})
const selectionQuantities = reactive<Record<string, number>>({})
const compositionError = ref('')
const composerGroupElements: Record<string, HTMLElement> = {}
const cart = usePublicCart()
const simulation = ref<Simulation>()
const scheduling = ref<SchedulingSlots>()
const schedulingMode = ref<'immediate' | 'scheduled'>('immediate')
const scheduledAtUtc = ref('')
const slotsLoading = ref(false)
const simulating = ref(false)
const priceChanged = ref(false)
const submitting = ref(false)
const confirmation = ref<Confirmation>()
const previousReference = ref<string | null>(null)
const idempotencyKey = ref<string>()
let controller: AbortController | undefined
let slotsController: AbortController | undefined

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
const pricingLabel = (strategy?: string) => ({
  Additive: 'valores somados', HighestPrice: 'considera o maior preço',
  Proportional: 'preço proporcional às frações', NoPriceChange: 'sem alteração de preço'
}[strategy ?? 'Additive'] ?? 'valores somados')
const sortedCategories = computed(() => [...(catalog.value?.categories ?? [])]
  .filter(category => category.isActive)
  .sort((a, b) => a.order - b.order))
const normalizedSearch = computed(() => catalogSearch.value.trim().toLocaleLowerCase('pt-BR'))
const visibleCategories = computed(() => sortedCategories.value
  .filter(category => activeCategoryId.value === null || category.id === activeCategoryId.value)
  .map(category => ({
    ...category,
    products: category.products.filter(product => {
      if (!product.isActive) return false
      if (!normalizedSearch.value) return true
      return [product.name, product.description, product.code]
        .some(value => value?.toLocaleLowerCase('pt-BR').includes(normalizedSearch.value))
    })
  }))
  .filter(category => !normalizedSearch.value || category.products.length > 0))
const visibleProductCount = computed(() => visibleCategories.value
  .reduce((total, category) => total + category.products.length, 0))
const availableServices = computed(() => context.value?.availability ?? [])
const selectedServiceAvailability = computed(() =>
  availableServices.value.find(item => item.serviceType === checkout.serviceType))
const serviceOptions = computed(() => {
  const options = context.value?.table
    ? [{ label: 'Nesta mesa', value: 'Table' as ServiceType }, { label: 'Retirada', value: 'Pickup' as ServiceType }, { label: 'Entrega', value: 'Delivery' as ServiceType }]
    : [{ label: 'Retirada', value: 'Pickup' as ServiceType }, { label: 'Entrega', value: 'Delivery' as ServiceType }]
  return options.map(option => ({
    ...option,
    disable: option.value === 'Table' && availableServices.value.find(item => item.serviceType === option.value)?.isAvailable === false
  }))
})
const activeMethods = computed(() => context.value?.paymentMethods ?? [])
const canCheckout = computed(() => cart.state.items.length > 0 &&
  (schedulingMode.value === 'scheduled'
    ? !!scheduledAtUtc.value && !!scheduling.value?.slots.some(slot => slot.startsAt === scheduledAtUtc.value)
    : selectedServiceAvailability.value?.isAvailable !== false))
const canChooseCheckout = computed(() => cart.state.items.length > 0 &&
  (selectedServiceAvailability.value?.isAvailable !== false ||
    (scheduling.value?.isEnabled === true && scheduling.value.slots.length > 0)))
const formatOpening = (value: string | null | undefined) => value
  ? new Date(value).toLocaleString('pt-BR')
  : null
const availabilityMessage = (reason: string) => ({
  CalendarException: 'A unidade está fechada excepcionalmente.',
  ServicePaused: 'Esta modalidade está temporariamente pausada.',
  OutsideBusinessHours: 'Estamos fora do horário de atendimento.',
  EstablishmentInactive: 'A unidade não está recebendo pedidos.'
}[reason] ?? 'Esta modalidade não está disponível agora.')

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
    const preferred = resolvedContext.table ? 'Table' : 'Pickup'
    checkout.serviceType = resolvedContext.availability?.find(item =>
      item.serviceType === preferred && item.isAvailable)?.serviceType ??
      resolvedContext.availability?.find(item => item.isAvailable &&
        (resolvedContext.table || item.serviceType !== 'Table'))?.serviceType ?? preferred
    checkout.paymentMethodId = resolvedContext.paymentMethods[0]?.id ?? ''
    applyPublicTheme(resolvedContext)
    await loadSchedulingSlots()
  } catch (failure) { error.value = failure } finally { loading.value = false }
}
async function loadSchedulingSlots() {
  slotsController?.abort()
  scheduling.value = undefined
  scheduledAtUtc.value = ''
  if (checkout.serviceType === 'Table') return
  slotsController = new AbortController()
  slotsLoading.value = true
  try {
    scheduling.value = await publicOrderingClient.scheduleSlots(slug.value, checkout.serviceType, slotsController.signal)
    if (!scheduling.value.isEnabled) schedulingMode.value = 'immediate'
  } catch (failure) {
    if (!(failure instanceof Error && failure.name === 'CanceledError')) error.value = failure
  } finally { slotsLoading.value = false }
}
const formatSlot = (value: string) => new Intl.DateTimeFormat('pt-BR', {
  timeZone: scheduling.value?.timeZoneId ?? 'UTC', dateStyle: 'medium', timeStyle: 'short'
}).format(new Date(value))
function openProduct(product: Product) {
  if (product.isAvailable === false) return
  selected.value = product
  quantity.value = 1
  variationId.value = product.variations.filter(x => x.isActive && x.isAvailable !== false)
    .sort((a, b) => a.order - b.order)[0]?.id ?? null
  notes.value = ''
  compositionError.value = ''
  Object.keys(selections).forEach(key => delete selections[key])
  Object.keys(selectionQuantities).forEach(key => delete selectionQuantities[key])
  product.additionalGroups.filter(x => x.isActive).forEach(group => { selections[group.id] = [] })
}
function toggle(groupId: string, additionalId: string, maximum: number) {
  const values = selections[groupId] ?? []
  const index = values.indexOf(additionalId)
  if (index >= 0) { values.splice(index, 1); delete selectionQuantities[`${groupId}:${additionalId}`] }
  else if (selectedCount(groupId) < maximum) { values.push(additionalId); selectionQuantities[`${groupId}:${additionalId}`] = 1 }
}
function isCompositeGroup(groupId: string) {
  const strategy = selected.value?.additionalGroups.find(group => group.id === groupId)?.pricingStrategy
  return strategy === 'HighestPrice' || strategy === 'Proportional'
}
function selectedCount(groupId: string) {
  const values = selections[groupId] ?? []
  return isCompositeGroup(groupId) ? values.length : values.reduce((sum, id) => sum + (selectionQuantities[`${groupId}:${id}`] ?? 1), 0)
}
function setSelectionQuantity(groupId: string, additionalId: string, value: string) {
  const parsed = Number(value)
  if (!Number.isFinite(parsed) || parsed < 1) return
  const max = selected.value?.additionalGroups.find(group => group.id === groupId)?.maximumSelection ?? 1
  const otherCount = selectedCount(groupId) - (selectionQuantities[`${groupId}:${additionalId}`] ?? 1)
  selectionQuantities[`${groupId}:${additionalId}`] = Math.min(Math.floor(parsed), max - otherCount)
}
function setComposerGroupElement(groupId: string, element: unknown) {
  if (element instanceof HTMLElement) composerGroupElements[groupId] = element
}
const composerGroups = computed(() => selected.value?.additionalGroups
  .filter(group => group.isActive).sort((a, b) => a.order - b.order) ?? [])
const requiredComposerGroups = computed(() => composerGroups.value
  .filter(group => group.minimumSelection > 0))
const completedComposerGroups = computed(() => requiredComposerGroups.value.filter(group => {
  const count = selectedCount(group.id)
  return count >= group.minimumSelection && count <= group.maximumSelection
}).length)
const composerUnitPrice = computed(() => {
  const product = selected.value
  if (!product) return 0
  const variation = product.variations.find(item => item.id === variationId.value)
  const chosen = product.additionalGroups.flatMap(group =>
    group.items.filter(item => selections[group.id]?.includes(item.id)).map(item => ({ group, item })))
  const compositionGroup = product.additionalGroups.find(group =>
    group.pricingStrategy === 'HighestPrice' || group.pricingStrategy === 'Proportional')
  const compositionOptions = chosen.filter(item => item.group.id === compositionGroup?.id)
  const base = compositionGroup?.pricingStrategy === 'HighestPrice'
    ? Math.max(...compositionOptions.map(item => item.item.price), 0)
    : compositionGroup?.pricingStrategy === 'Proportional'
      ? compositionOptions.reduce((sum, item) => sum + item.item.price /
        Math.max(compositionOptions.length, 1), 0)
      : variation?.price ?? product.basePrice
  return base + chosen
    .filter(item => (item.group.pricingStrategy ?? 'Additive') === 'Additive')
    .reduce((sum, item) => sum + item.item.price *
      (selectionQuantities[`${item.group.id}:${item.item.id}`] ?? 1), 0)
})
async function addProduct() {
  const product = selected.value
  if (!product || product.isAvailable === false) return
  const invalid = product.additionalGroups.filter(group => group.isActive).find(group => {
    const count = selectedCount(group.id)
    return count < group.minimumSelection || count > group.maximumSelection
  })
  if (invalid) {
    compositionError.value = invalid.name + ': selecione entre ' +
      invalid.minimumSelection + ' e ' + invalid.maximumSelection + '.'
    await nextTick()
    composerGroupElements[invalid.id]?.focus()
    return
  }
  const chosenPairs = new Set(product.additionalGroups.flatMap(group =>
    (selections[group.id] ?? []).map(optionId => `${group.id}:${optionId}`)))
  for (const group of product.additionalGroups) for (const option of group.items) {
    if (!chosenPairs.has(`${group.id}:${option.id}`)) continue
    for (const rule of option.compatibilityRules ?? []) {
      const targetSelected = chosenPairs.has(`${rule.targetGroupId}:${rule.targetAdditionalId}`)
      if (rule.kind === 'Requires' && !targetSelected) {
        compositionError.value = `${option.name} também exige uma opção de outro grupo.`
        return
      }
      if (rule.kind === 'Excludes' && targetSelected) {
        compositionError.value = `${option.name} não pode ser combinada com a opção selecionada.`
        return
      }
    }
  }
  const variation = product.variations.find(item => item.id === variationId.value)
  const additionals = product.additionalGroups.flatMap(group =>
    group.items.filter(item => selections[group.id]?.includes(item.id)).map(item => ({ group, item }))
  )
  if (variation?.isAvailable === false || additionals.some(({ item }) => item.isAvailable === false)) {
    compositionError.value = 'Uma opção selecionada ficou indisponível. Revise a composição.'
    return
  }
  const compositionGroup = product.additionalGroups.find(group => group.pricingStrategy === 'HighestPrice' || group.pricingStrategy === 'Proportional')
  const compositionOptions = additionals.filter(x => x.group.id === compositionGroup?.id)
  const compositionPrice = compositionGroup?.pricingStrategy === 'HighestPrice'
    ? Math.max(...compositionOptions.map(x => x.item.price), 0)
    : compositionGroup?.pricingStrategy === 'Proportional'
      ? compositionOptions.reduce((sum, x) => sum + x.item.price / Math.max(compositionOptions.length, 1), 0)
      : (variation?.price ?? product.basePrice)
  const additivePrice = additionals.filter(x => (x.group.pricingStrategy ?? 'Additive') === 'Additive').reduce((sum, x) => sum + x.item.price * (selectionQuantities[`${x.group.id}:${x.item.id}`] ?? 1), 0)
  cart.add({
    key: crypto.randomUUID(), productId: product.id, variationId: variation?.id ?? null,
    productName: product.name, variationName: variation?.name ?? null,
    displayedUnitPrice: compositionPrice + additivePrice,
    quantity: quantity.value, notes: notes.value.trim() || null,
    additionals: additionals.map(({ group, item }) => {
      const fractional = group.pricingStrategy === 'HighestPrice' || group.pricingStrategy === 'Proportional'
      const count = additionals.filter(x => x.group.id === group.id).length
      return { additionalId: item.id, quantity: fractional ? 1 : selectionQuantities[`${group.id}:${item.id}`] ?? 1, groupId: group.id,
        portionNumerator: fractional ? 1 : null, portionDenominator: fractional ? count : null }
    })
  })
  selected.value = undefined
}
function request(deliveryAddress: Address | null = null, scheduledAtOverride?: string | null) {
  return {
    serviceType: checkout.serviceType,
    customerId: null,
    customerAddressId: null,
    tableToken: checkout.serviceType === 'Table' ? context.value?.table?.token ?? null : null,
    deliveryAddress,
    couponCode: checkout.couponCode.trim() || null,
    paymentMethodId: checkout.paymentMethodId || null,
    items: orderItems(cart.state.items),
    scheduledAtUtc: scheduledAtOverride !== undefined ? scheduledAtOverride :
      schedulingMode.value === 'scheduled' ? scheduledAtUtc.value : null
  }
}
async function simulate() {
  if (!cart.state.items.length) return
  simulating.value = true
  error.value = null
  try {
    const fallbackSlot = selectedServiceAvailability.value?.isAvailable === false && scheduling.value?.slots.length
      ? scheduling.value.slots[0]?.startsAt ?? null : undefined
    const result = await publicOrderingClient.simulate(
      slug.value,
      request(checkout.serviceType === 'Delivery' ? address : null, fallbackSlot)
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
    const deliveryQuoteChanged = checkout.serviceType === 'Delivery' && simulation.value && (
      simulation.value.deliveryRegionId !== finalSimulation.deliveryRegionId ||
      simulation.value.deliveryFee !== finalSimulation.deliveryFee ||
      simulation.value.deliveryEstimatedMinutes !== finalSimulation.deliveryEstimatedMinutes
    )
    if (simulation.value && (simulation.value.total !== finalSimulation.total || deliveryQuoteChanged)) {
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
      receivedAmount: checkout.receivedAmount,
      deliveryRegionId: finalSimulation.deliveryRegionId,
      expectedDeliveryFee: finalSimulation.deliveryFee,
      expectedDeliveryEstimatedMinutes: finalSimulation.deliveryEstimatedMinutes,
      deliveryQuoteIssuedAt: finalSimulation.deliveryQuoteIssuedAt
    }, idempotencyKey.value)
    confirmation.value = result
    receiptStorage.save(slug.value, result.reference)
    cart.clear()
    step.value = 'receipt'
    idempotencyKey.value = undefined
  } catch (failure) {
    if (failure instanceof ApiError && failure.problem.status === 409 && schedulingMode.value === 'scheduled') {
      await loadSchedulingSlots()
      scheduledAtUtc.value = ''
      error.value = new Error('Esse horário acabou de ficar indisponível. O carrinho foi preservado; escolha outro horário.')
      return
    }
    error.value = failure instanceof ApiError && typeof failure.problem.currentTotal === 'number'
      ? new Error(`${failure.message} Total atualizado: ${money(failure.problem.currentTotal)}.`)
      : failure
  } finally { submitting.value = false }
}
function editIntent() {
  idempotencyKey.value = undefined
  priceChanged.value = false
}
watch(() => [cart.state.revision, checkout.serviceType, checkout.couponCode,
  checkout.paymentMethodId, checkout.receivedAmount, customer.name, customer.phone,
  customer.email, ...Object.values(address)], editIntent)
watch(() => checkout.serviceType, async () => {
  schedulingMode.value = 'immediate'
  await loadSchedulingSlots()
})
watch(() => [slug.value, tableToken.value], load)
onMounted(load)
onBeforeUnmount(() => { controller?.abort(); slotsController?.abort() })
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
      <PublicUnitContext
        :context="context"
      />
      <aside v-if="step === 'catalog' && previousReference" class="resume-order">
        <span>Você tem um pedido recente.</span>
        <q-btn flat label="Retomar acompanhamento"
          @click="router.push('/order/track/' + previousReference)" />
      </aside>
      <ProblemBanner :error="error">
        <q-btn v-if="step !== 'catalog'" flat label="Recalcular" @click="simulate" />
      </ProblemBanner>
      <q-banner v-if="selectedServiceAvailability?.isAvailable === false" class="bg-amber-1 text-brown-9 q-mb-md" role="status">
        <strong>{{ selectedServiceAvailability.message || availabilityMessage(selectedServiceAvailability.reason) }}</strong>
        <span v-if="selectedServiceAvailability.nextOpening">
          Próxima abertura: {{ formatOpening(selectedServiceAvailability.nextOpening) }}.
        </span>
      </q-banner>

      <main v-if="step === 'catalog'" aria-label="Cardápio">
        <div class="catalog-controls">
          <PublicCatalogSearch v-model="catalogSearch" />
          <PublicCategoryNavigation v-model="activeCategoryId" :categories="sortedCategories" />
        </div>
        <section v-for="category in visibleCategories" :key="category.id" class="category">
          <h2>{{ category.name }}</h2><p v-if="category.description">{{ category.description }}</p>
          <div class="product-grid">
            <PublicProductCard
              v-for="product in category.products"
              :key="product.id"
              :product="product"
              :formatted-price="money(product.basePrice)"
              @select="openProduct"
            />
          </div>
        </section>
        <div v-if="visibleProductCount === 0" class="state-panel" role="status">
          <p>{{ normalizedSearch ? 'Nenhum produto encontrado para esta busca.' : 'Nenhum item disponível no momento.' }}</p>
          <q-btn v-if="normalizedSearch" flat label="Limpar busca" @click="catalogSearch = ''" />
          <q-btn v-else flat label="Atualizar cardápio" @click="load" />
        </div>
      </main>
      <PublicCartAccess
        v-if="step === 'catalog'"
        :count="cart.count.value"
        :total="money(cart.displayedTotal.value)"
        :disabled="cart.state.items.length === 0"
        @open="step = 'cart'; simulate()"
      />

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
          <span v-if="checkout.serviceType === 'Delivery' && simulation.deliveryEstimatedMinutes">
            Entrega estimada <b>{{ simulation.deliveryEstimatedMinutes }} min</b>
          </span>
          <span v-if="checkout.serviceType === 'Delivery' && simulation.deliveryRegionName" class="text-grey-7">
            Cobertura: {{ simulation.deliveryRegionName }}
          </span>
          <span class="grand-total">Total atualizado <b>{{ money(simulation.total) }}</b></span>
          <p v-if="priceChanged" role="alert">O total mudou. Confira os valores atualizados antes de confirmar.</p>
        </div>
        <div class="flow-actions"><q-btn flat label="Voltar ao cardápio" @click="step = 'catalog'" />
          <q-btn label="Continuar" color="primary" :disable="!simulation || !canChooseCheckout"
            @click="step = 'checkout'" /></div>
      </main>

      <main v-else-if="step === 'checkout'" class="flow-panel">
        <h2>Finalizar pedido</h2>
        <q-form @submit="confirm">
          <fieldset><legend>Como você quer receber?</legend>
            <q-option-group v-model="checkout.serviceType" :options="serviceOptions" type="radio" />
          </fieldset>
          <fieldset v-if="checkout.serviceType !== 'Table' && scheduling?.isEnabled">
            <legend>Quando prefere receber?</legend>
            <q-option-group v-model="schedulingMode" :options="[
              { label: 'O mais rápido possível', value: 'immediate', disable: selectedServiceAvailability?.isAvailable === false },
              { label: 'Agendar', value: 'scheduled' }
            ]" type="radio" />
            <div v-if="schedulingMode === 'scheduled'">
              <div v-if="slotsLoading" role="status">Buscando horários disponíveis…</div>
              <q-select v-else v-model="scheduledAtUtc" outlined emit-value map-options
                label="Horário disponível" :options="scheduling.slots.map(slot => ({ label: formatSlot(slot.startsAt), value: slot.startsAt }))" />
              <small>Horários no fuso {{ scheduling.timeZoneId }} · pedidos com pelo menos {{ scheduling.minimumAdvanceMinutes }} minutos de antecedência.</small>
            </div>
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
              :loading="submitting" :disable="submitting || !canCheckout" /></div>
        </q-form>
      </main>

      <main v-else class="flow-panel receipt" aria-live="polite">
        <h2>Pedido confirmado</h2>
        <p>Pedido nº <strong>{{ confirmation?.number }}</strong></p>
        <p>Total {{ money(confirmation?.total ?? 0) }}</p>
        <p class="public-reference">Referência: {{ confirmation?.reference }}</p>
        <q-btn label="Acompanhar pedido" color="primary"
          @click="router.push('/order/track/' + confirmation?.reference)" />
      </main>
    </template>

    <q-dialog :model-value="!!selected" @update:model-value="value => { if (!value) selected = undefined }">
      <q-card v-if="selected" class="composition-card">
        <q-card-section class="composer-summary">
          <p class="eyebrow">Monte seu item</p>
          <h2>{{ selected.name }}</h2>
          <p>{{ selected.description }}</p>
          <p v-if="requiredComposerGroups.length" role="status">
            {{ completedComposerGroups }} de {{ requiredComposerGroups.length }} requisitos concluídos
          </p>
        </q-card-section>
        <q-card-section class="composition-body">
          <fieldset v-if="selected.variations.filter(x => x.isActive).length"><legend>Escolha uma opção · obrigatório</legend>
            <label v-for="variation in selected.variations.filter(x => x.isActive).sort((a,b) => a.order-b.order)" :key="variation.id">
              <input v-model="variationId" type="radio" :value="variation.id" :disabled="variation.isAvailable === false">
              {{ variation.name }} — {{ money(variation.price) }}<small v-if="variation.isAvailable === false"> · indisponível</small>
            </label>
          </fieldset>
          <fieldset v-for="group in composerGroups" :id="`composer-group-${group.id}`" :key="group.id"
            :ref="element => setComposerGroupElement(group.id, element)" tabindex="-1">
            <legend>
              {{ group.name }} · {{ group.minimumSelection > 0 ? 'obrigatório' : 'opcional' }}
              ({{ selectedCount(group.id) }}/{{ group.maximumSelection }}) · {{ pricingLabel(group.pricingStrategy) }}
            </legend>
            <label v-for="item in group.items.filter(x => x.isActive).sort((a,b) => a.order-b.order)" :key="item.id">
              <input type="checkbox" :checked="selections[group.id]?.includes(item.id)"
                :disabled="item.isAvailable === false || (!selections[group.id]?.includes(item.id) && selectedCount(group.id) >= group.maximumSelection)"
                @change="toggle(group.id, item.id, group.maximumSelection)">
              {{ item.name }} <span>{{ (group.pricingStrategy ?? 'Additive') === 'NoPriceChange' ? 'sem alteração' : money(item.price) }}</span><small v-if="item.isAvailable === false"> · indisponível</small>
              <input v-if="(group.pricingStrategy ?? 'Additive') === 'Additive' && selections[group.id]?.includes(item.id)" type="number" min="1" :max="group.maximumSelection" :value="selectionQuantities[`${group.id}:${item.id}`] ?? 1" aria-label="Quantidade da opção" @click.stop @input="setSelectionQuantity(group.id, item.id, ($event.target as HTMLInputElement).value)">
            </label>
          </fieldset>
          <q-input v-if="selected.allowsNotes" v-model="notes" label="Observações" type="textarea" />
          <q-input v-model.number="quantity" label="Quantidade" type="number" min="1" />
          <p v-if="compositionError" role="alert" class="text-negative">{{ compositionError }}</p>
        </q-card-section>
        <q-card-actions align="right" class="composer-actions">
          <div class="composer-total">
            <span>{{ quantity }} × {{ money(composerUnitPrice) }}</span>
            <strong>{{ money(composerUnitPrice * Math.max(quantity, 1)) }}</strong>
          </div>
          <q-btn flat label="Cancelar" @click="selected = undefined" />
          <q-btn color="primary" label="Adicionar" @click="addProduct" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
