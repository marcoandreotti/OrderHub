import type { RouteRecordRaw } from 'vue-router'

export const routes: RouteRecordRaw[] = [
  {
    path: '/order/track/:reference',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      { path: '', component: () => import('../modules/public-ordering/TrackingPage.vue') }
    ]
  },
  {
    path: '/order/:slug/table/:tableToken',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      { path: '', component: () => import('../modules/public-ordering/PublicOrderingPage.vue') }
    ]
  },
  {
    path: '/order/:slug',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      { path: '', component: () => import('../modules/public-ordering/PublicOrderingPage.vue') }
    ]
  },
  {
    path: '/login',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      { path: '', component: () => import('../modules/session/LoginPage.vue') }
    ]
  },
  {
    path: '/change-password',
    component: () => import('../layouts/PublicLayout.vue'),
    meta: { requiresSession: true },
    children: [
      {
        path: '',
        component: () => import('../modules/session/ChangePasswordPage.vue')
      }
    ]
  },
  {
    path: '/access-denied',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      {
        path: '',
        component: () => import('../modules/session/AccessDeniedPage.vue')
      }
    ]
  },
  {
    path: '/',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      { path: '', component: () => import('../pages/FoundationPage.vue') }
    ]
  },
  {
    path: '/operations',
    component: () => import('../layouts/OperationsLayout.vue'),
    children: [
      { path: '', component: () => import('../pages/FoundationPage.vue') }
    ]
  },
  {
    path: '/administration',
    meta: { requiresSession: true, capability: 'management' },
    component: () => import('../layouts/AdministrationLayout.vue'),
    children: [
      {
        path: '',
        component: () => import('../modules/administration/HomePage.vue')
      },
      {
        path: 'users',
        meta: { capability: 'administration' },
        component: () => import('../modules/administration/users/UsersPage.vue')
      },
      {
        path: 'catalog',
        meta: { capability: 'management' },
        component: () =>
          import('../modules/administration/catalog/CatalogPage.vue')
      },
      {
        path: 'customers',
        meta: { capability: 'customer-operations' },
        component: () =>
          import('../modules/administration/customers/CustomersPage.vue')
      },
      {
        path: 'coupons',
        meta: { capability: 'promotion-management' },
        component: () =>
          import('../modules/administration/coupons/CouponsPage.vue')
      },
      {
        path: 'payment-methods',
        meta: { capability: 'payment-management' },
        component: () =>
          import('../modules/administration/payment-methods/PaymentMethodsPage.vue')
      },
      {
        path: 'foundation',
        component: () => import('../pages/FoundationPage.vue')
      }
    ]
  }
]
