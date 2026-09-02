import { boot } from 'quasar/wrappers'
import axios, { AxiosError } from 'axios'

export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  traceId?: string
  errors?: Record<string, string[]>
}

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080'
})

api.interceptors.request.use(config => {
  config.headers.set('X-Correlation-ID', crypto.randomUUID())
  return config
})

api.interceptors.response.use(
  response => response,
  (error: AxiosError<ProblemDetails>) => Promise.reject(error.response?.data ?? error)
)

export default boot(({ app }) => {
  app.config.globalProperties.$api = api
})
