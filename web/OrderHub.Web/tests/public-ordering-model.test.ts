import axios, { AxiosError, AxiosHeaders, type InternalAxiosRequestConfig } from 'axios'
import { beforeEach, describe, expect, it } from 'vitest'
import { hydrateCartFromCatalog, loadCart, usePublicCart } from '../src/modules/public-ordering/cart'
import { createPublicOrderingClient } from '../src/modules/public-ordering/client'
import { applyPublicTheme, contrast } from '../src/modules/public-ordering/theme'
import { pollingDelay, terminalOrderStatuses } from '../src/modules/public-ordering/tracking'
import { checkoutValidation } from '../src/modules/public-ordering/checkout'

beforeEach(() => localStorage.clear())

describe('carrinho público versionado', () => {
  it('restaura somente a intenção da mesma unidade', () => {
    loadCart('unit-a')
    usePublicCart().add({
      key: 'line-1', productId: 'product', variationId: null, quantity: 2,
      notes: 'sem cebola', additionals: [], productName: 'Pizza',
      variationName: null, displayedUnitPrice: 20
    })
    const persisted = localStorage.getItem('orderhub.public-cart.unit-a')!
    expect(persisted).not.toMatch(/displayedUnitPrice|productName|variationName/)
    loadCart('unit-b')
    expect(usePublicCart().state.items).toHaveLength(0)
    loadCart('unit-a')
    hydrateCartFromCatalog({
      establishmentId: 'ignored', establishmentName: 'Unit', slug: 'unit-a',
      categories: [{
        id: 'category', parentId: null, name: 'Pizzas', description: null,
        order: 0, imageUrl: null, isActive: true,
        products: [{
          id: 'product', code: 'P', name: 'Pizza', description: null,
          basePrice: 20, isFeatured: false, isActive: true, allowsNotes: true,
          images: [], variations: [], additionalGroups: []
        }]
      }]
    })
    expect(usePublicCart().state.items[0]).toMatchObject({
      productId: 'product', productName: 'Pizza', displayedUnitPrice: 20,
      quantity: 2, notes: 'sem cebola'
    })
  })

  it('descarta estado incompatível ou inválido', () => {
    localStorage.setItem('orderhub.public-cart.unit', JSON.stringify({
      version: 0, slug: 'unit', items: [{ productId: 'old' }]
    }))
    loadCart('unit')
    expect(usePublicCart().state.items).toHaveLength(0)
    expect(localStorage.getItem('orderhub.public-cart.unit')).toBeNull()
  })
})

describe('cliente HTTP público', () => {
  it('usa somente slug/token e não transporta TenantId', async () => {
    const http = axios.create()
    let request: InternalAxiosRequestConfig | undefined
    http.defaults.adapter = async config => {
      request = config
      return {
        config, status: 200, statusText: 'OK', headers: new AxiosHeaders(),
        data: { establishmentName: 'Unit', slug: 'unit', theme: {}, table: null, paymentMethods: [] }
      }
    }
    await createPublicOrderingClient(http).context('unit', 'opaque-token')
    expect(request?.url).toBe('/api/public/ordering/unit/context')
    expect(request?.params).toEqual({ tableToken: 'opaque-token' })
    expect(JSON.stringify(request)).not.toMatch(/tenantId/i)
  })

  it('converte falha HTTP em ProblemDetails preservando a explicação', async () => {
    const http = axios.create()
    http.defaults.adapter = async config => {
      const response = {
        config, status: 409, statusText: 'Conflict', headers: new AxiosHeaders(),
        data: { title: 'Oferta alterada', detail: 'Recalcule o pedido.' }
      }
      throw new AxiosError('Conflict', 'ERR_BAD_REQUEST', config, undefined, response)
    }
    await expect(createPublicOrderingClient(http).catalog('unit')).rejects.toMatchObject({
      message: 'Recalcule o pedido.'
    })
  })
})

it('mantém contraste mínimo ao aplicar tema público inseguro', () => {
  applyPublicTheme({
    establishmentName: 'Unit', slug: 'unit', table: null, paymentMethods: [],
    theme: {
      primaryColor: '#ffffff', secondaryColor: '#ffffff',
      backgroundColor: '#ffffff', textColor: '#ffffff',
      fontFamily: '<script>', logoUrl: null
    }
  })
  const style = document.documentElement.style
  expect(contrast(style.getPropertyValue('--oh-color-primary'), '#ffffff')).toBeGreaterThanOrEqual(4.5)
  expect(contrast(style.getPropertyValue('--oh-color-text'), '#ffffff')).toBeGreaterThanOrEqual(4.5)
  expect(style.getPropertyValue('--oh-font-family')).toBe('system-ui, sans-serif')
})

it('aplica backoff limitado e reconhece estados terminais do acompanhamento', () => {
  expect([0, 1, 2, 8].map(pollingDelay)).toEqual([5000, 10000, 20000, 60000])
  expect(terminalOrderStatuses.has('Completed')).toBe(true)
  expect(terminalOrderStatuses.has('Preparing')).toBe(false)
})

it('valida dados necessários para mesa, retirada e entrega', () => {
  const emptyAddress = {
    label: 'Principal', street: '', number: '', complement: null,
    neighborhood: '', city: '', state: '', postalCode: ''
  }
  const validAddress = {
    ...emptyAddress, street: 'Rua A', number: '10', neighborhood: 'Centro',
    city: 'São Paulo', state: 'SP', postalCode: '01001000'
  }
  expect(checkoutValidation('Table', 'pay', { name: '', phone: '' }, emptyAddress)).toBe('')
  expect(checkoutValidation('Pickup', 'pay', { name: '', phone: '' }, emptyAddress)).toContain('nome')
  expect(checkoutValidation('Pickup', 'pay', { name: 'Ana', phone: '1199' }, emptyAddress)).toBe('')
  expect(checkoutValidation('Delivery', 'pay', { name: 'Ana', phone: '1199' }, emptyAddress)).toContain('endereço')
  expect(checkoutValidation('Delivery', 'pay', { name: 'Ana', phone: '1199' }, validAddress)).toBe('')
  expect(checkoutValidation('Table', '', { name: '', phone: '' }, emptyAddress)).toContain('pagamento')
})
