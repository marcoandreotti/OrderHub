<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useSessionStore } from '../modules/session/store'
const router = useRouter()
const session = useSessionStore()
async function logout() { try { await session.logout() } finally { await router.replace('/login') } }
</script>
<template>
  <q-layout view="hHh lpR fFf">
    <q-header bordered class="bg-white text-dark">
      <a class="skip-link" href="#platform-content">Ir para o conteúdo</a>
      <q-toolbar class="q-px-md q-py-sm">
        <q-toolbar-title class="brand-wordmark">OrderHub <span>PLATFORM</span></q-toolbar-title>
        <q-btn flat no-caps label="Sair" @click="logout" />
      </q-toolbar>
    </q-header>
    <q-page-container><main id="platform-content" tabindex="-1"><router-view /></main></q-page-container>
  </q-layout>
</template>
