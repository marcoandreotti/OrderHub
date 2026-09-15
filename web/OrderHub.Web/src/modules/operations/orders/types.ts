export type OrderStatus =
  | 'Confirmed'
  | 'Preparing'
  | 'Ready'
  | 'OutForDelivery'
  | 'Completed'
  | 'Cancelled'
  | 'Rejected'

export type OrderServiceType = 'Table' | 'Pickup' | 'Delivery'

export interface OrderSummary {
  id: string
  number: number
  serviceType: OrderServiceType
  status: OrderStatus
  customerName: string | null
  customerPhone: string | null
  total: number
  createdAt: string
}

export interface OrderAdditional {
  name: string
  unitPrice: number
  quantity: number
}

export interface OrderItem {
  id: string
  productName: string
  variationName: string | null
  unitPrice: number
  quantity: number
  total: number
  notes: string | null
  additionals: OrderAdditional[]
}

export interface OrderHistory {
  previousStatus: OrderStatus
  newStatus: OrderStatus
  occurredAt: string
  actorId: string | null
  note: string | null
}

export interface OrderDetail {
  id: string
  number: number | null
  publicReference: string | null
  serviceType: OrderServiceType
  status: OrderStatus
  customerName: string | null
  customerPhone: string | null
  tableCode: string | null
  subtotal: number
  discount: number
  fees: number
  total: number
  couponCode: string | null
  confirmedAmount: number
  isFullyPaid: boolean
  items: OrderItem[]
  history: OrderHistory[]
}

export interface OrderPage {
  page: number
  pageSize: number
  total: number
  items: OrderSummary[]
}

export interface OrderFilters {
  status?: OrderStatus
  serviceType?: OrderServiceType
  number?: number
}
