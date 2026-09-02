import type { RouteRecordRaw } from 'vue-router'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [{ path: '', component: () => import('../pages/FoundationPage.vue') }]
  },
  {
    path: '/operations',
    component: () => import('../layouts/OperationsLayout.vue'),
    children: [{ path: '', component: () => import('../pages/FoundationPage.vue') }]
  },
  {
    path: '/administration',
    component: () => import('../layouts/AdministrationLayout.vue'),
    children: [{ path: '', component: () => import('../pages/FoundationPage.vue') }]
  }
]
