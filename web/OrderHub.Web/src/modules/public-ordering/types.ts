export interface PublicTheme {
  primaryColor: string
  secondaryColor: string
  backgroundColor: string
  textColor: string
  fontFamily: string
  logoUrl: string | null
}
export interface PublicPaymentMethod { id: string; code: string; name: string; isOnline: boolean; allowsChange: boolean }
export interface PublicContext {
  establishmentName: string; slug: string; theme: PublicTheme
  table: { code: string; token: string } | null
  paymentMethods: PublicPaymentMethod[]
}
export interface Additional { id: string; name: string; price: number; isActive: boolean; order: number }
export interface AdditionalGroup {
  id: string; name: string; minimumSelection: number; maximumSelection: number
  isActive: boolean; order: number; items: Additional[]
}
export interface ProductVariation { id: string; name: string; price: number; order: number; isActive: boolean }
export interface Product {
  id: string; code: string; name: string; description: string | null
  basePrice: number; isFeatured: boolean; isActive: boolean; allowsNotes: boolean
  images: { id: string; url: string; order: number; isPrincipal: boolean }[]
  variations: ProductVariation[]; additionalGroups: AdditionalGroup[]
}
export interface PublicCatalog {
  establishmentId: string; establishmentName: string; slug: string
  categories: {
    id: string; parentId: string | null; name: string; description: string | null
    order: number; imageUrl: string | null; isActive: boolean; products: Product[]
  }[]
}
export interface Address {
  label: string; street: string; number: string; complement: string | null
  neighborhood: string; city: string; state: string; postalCode: string
}
export interface OrderItemRequest {
  productId: string; variationId: string | null; quantity: number; notes: string | null
  additionals: { additionalId: string; quantity: number }[]
}
export type ServiceType = 'Table' | 'Pickup' | 'Delivery'
export interface SimulationRequest {
  serviceType: ServiceType; customerId: string | null; customerAddressId: string | null
  tableToken: string | null; deliveryAddress: Address | null; couponCode: string | null
  paymentMethodId: string | null; items: OrderItemRequest[]
}
export interface SimulatedItem {
  productName: string; variationName: string | null; unitPrice: number; quantity: number
  total: number; additionals: { name: string; unitPrice: number; quantity: number }[]
}
export interface Simulation {
  subtotal: number; discount: number; fees: number; total: number
  couponCode: string | null; items: SimulatedItem[]
}
export interface ConfirmationRequest extends Omit<SimulationRequest, 'paymentMethodId'> {
  paymentMethodId: string; receivedAmount: number | null
}
export interface Confirmation { reference: string; number: number; status: string; total: number }
export interface Tracking extends Confirmation {
  serviceType: ServiceType; subtotal: number; discount: number; fees: number
  couponCode: string | null; items: SimulatedItem[]
  history: { status: string; occurredAt: string; note: string | null }[]
}
