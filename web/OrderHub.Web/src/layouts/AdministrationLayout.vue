<script setup lang="ts">
import { nextTick, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '../modules/session/store'
const drawer = ref(false)
const session = useSessionStore()
const router = useRouter()
const route = useRoute()
watch(
  () => route.path,
  async () => {
    drawer.value = false
    await nextTick()
    document.getElementById('admin-content')?.focus()
  }
)
async function logout() {
  try {
    await session.logout()
  } finally {
    await router.replace('/login')
  }
}
</script>
<template>
  <q-layout view="hHh lpR fFf">
    <q-header bordered class="bg-white text-dark">
      <a class="skip-link" href="#admin-content">Ir para o conteúdo</a>
      <q-toolbar class="q-px-md q-py-sm">
        <q-btn flat round aria-label="Abrir navegação" @click="drawer = !drawer"
          ><span aria-hidden="true">☰</span></q-btn
        >
        <q-toolbar-title class="brand-wordmark"
          >OrderHub <span>ADMIN</span></q-toolbar-title
        >
        <q-btn flat no-caps label="Sair" @click="logout" />
      </q-toolbar>
    </q-header>
    <q-drawer
      v-model="drawer"
      show-if-above
      bordered
      :width="256"
      class="bg-white"
    >
      <nav aria-label="Administração" class="q-pa-md">
        <div class="text-overline text-grey-7 q-mb-md">ÁREA ADMINISTRATIVA</div>
        <q-select
          :model-value="session.unitId"
          :options="session.units"
          option-value="id"
          option-label="name"
          emit-value
          map-options
          outlined
          label="Unidade ativa"
          :disable="!session.units.length"
          @update:model-value="session.selectUnit"
          class="q-mb-lg"
        />
        <q-list>
          <q-item
            v-if="session.can('management')"
            clickable
            to="/administration/catalog"
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Catálogo</q-item-section></q-item
          >
          <q-item
            v-if="session.can('administration')"
            clickable
            to="/administration/users"
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Usuários</q-item-section></q-item
          >
          <q-item
            v-if="session.can('customer-operations')"
            clickable
            to="/administration/customers"
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Clientes</q-item-section></q-item
          >
          <q-item
            v-if="session.can('promotion-management')"
            clickable
            to="/administration/coupons"
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Cupons</q-item-section></q-item
          >
          <q-item
            v-if="session.can('payment-management')"
            clickable
            to="/administration/payment-methods"
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Formas de pagamento</q-item-section></q-item
          >
          <q-item
            v-if="session.can('management')"
            clickable
            to="/administration"
            exact
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Visão geral</q-item-section></q-item
          >
          <q-item
            v-if="session.can('management')"
            clickable
            to="/administration/foundation"
            active-class="bg-indigo-1 text-primary"
            class="rounded-borders"
            ><q-item-section>Fundação do projeto</q-item-section></q-item
          >
        </q-list>
      </nav>
    </q-drawer>
    <q-page-container
      ><main id="admin-content" tabindex="-1">
        <router-view v-if="session.context" :key="session.revision" /></main
    ></q-page-container>
  </q-layout>
</template>
