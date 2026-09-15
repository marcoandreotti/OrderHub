<script setup lang="ts">
import { nextTick, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSessionStore } from '../modules/session/store'

const session = useSessionStore()
const router = useRouter()
const route = useRoute()

watch(
  () => route.fullPath,
  async () => {
    await nextTick()
    document.getElementById('operations-content')?.focus()
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
      <a class="skip-link" href="#operations-content">Ir para o conteúdo</a>
      <q-toolbar class="q-px-md q-py-sm operations-toolbar">
        <q-toolbar-title class="brand-wordmark">
          OrderHub <span>OPERAÇÃO</span>
        </q-toolbar-title>
        <q-select
          :model-value="session.unitId"
          :options="session.units"
          option-value="id"
          option-label="name"
          emit-value
          map-options
          dense
          outlined
          label="Unidade ativa"
          aria-label="Unidade ativa"
          :disable="!session.units.length"
          @update:model-value="session.selectUnit"
          class="operations-unit"
        />
        <q-btn
          v-if="session.can('management')"
          flat
          no-caps
          label="Administração"
          to="/administration"
        />
        <q-btn flat no-caps label="Sair" @click="logout" />
      </q-toolbar>
    </q-header>
    <q-page-container>
      <main id="operations-content" tabindex="-1">
        <router-view v-if="session.context" :key="session.revision" />
      </main>
    </q-page-container>
  </q-layout>
</template>
