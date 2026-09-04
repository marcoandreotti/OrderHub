import axios, { type AxiosError, type AxiosInstance } from 'axios'
import { ApiError, type ProblemDetails } from '../../http/client'
import type { Address, Confirmation, ConfirmationRequest, PublicCatalog, PublicContext, Simulation, SimulationRequest, Tracking } from './types'

const path = (slug: string) => '/api/public/ordering/' + encodeURIComponent(slug)

export function createPublicOrderingClient(http: AxiosInstance) {
  http.interceptors.response.use(undefined, (error: AxiosError<ProblemDetails>) => {
    if (axios.isCancel(error)) throw error
    throw new ApiError({ ...error.response?.data, status: error.response?.status })
  })
  return {
  async context(slug: string, tableToken?: string, signal?: AbortSignal) {
    return (await http.get<PublicContext>(path(slug) + '/context', {
      params: tableToken ? { tableToken } : undefined, signal
    })).data
  },
  async catalog(slug: string, signal?: AbortSignal) {
    return (await http.get<PublicCatalog>(
      '/api/public/establishments/' + encodeURIComponent(slug) + '/catalog', { signal }
    )).data
  },
  async customer(slug: string, value: {
    name: string; phone: string; email: string | null; address: Address | null
  }, signal?: AbortSignal) {
    return (await http.post<{ customerId: string; addressId: string | null }>(
      path(slug) + '/customers', value, { signal }
    )).data
  },
  async simulate(slug: string, value: SimulationRequest, signal?: AbortSignal) {
    return (await http.post<Simulation>(path(slug) + '/simulate', value, { signal })).data
  },
  async confirm(slug: string, value: ConfirmationRequest, key: string) {
    return (await http.post<Confirmation>(path(slug) + '/orders', value, {
      headers: { 'Idempotency-Key': key }
    })).data
  },
  async track(reference: string, signal?: AbortSignal) {
    return (await http.get<Tracking>(
      '/api/public/ordering/orders/' + encodeURIComponent(reference), { signal }
    )).data
  },
  async cancel(reference: string, reason: string | null) {
    await http.post(
      '/api/public/ordering/orders/' + encodeURIComponent(reference) + '/cancel',
      { reason }
    )
  }
  }
}

export const publicOrderingClient = createPublicOrderingClient(
  axios.create({ baseURL: import.meta.env.VITE_API_BASE_URL ?? '' })
)
