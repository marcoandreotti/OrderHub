<script setup lang="ts">
import { computed } from 'vue'
import { useSessionStore } from '../session/store'
const session = useSessionStore()
const unit = computed(() =>
  session.units.find((item) => item.id === session.unitId)
)
</script>
<template>
  <q-page padding class="admin-page">
    <div class="text-overline text-primary">SEU ESTABELECIMENTO</div>
    <h1 class="text-h4 q-mt-sm q-mb-sm">Visão geral</h1>
    <p class="text-grey-8 q-mb-xl">
      {{
        unit
          ? `Administrando ${unit.name}.`
          : 'Nenhuma unidade autorizada disponível.'
      }}
    </p>
    <q-banner v-if="!unit" class="bg-amber-1 rounded-borders"
      >Solicite uma associação de unidade ao responsável pelo seu
      acesso.</q-banner
    >
    <q-card v-else flat bordered
      ><q-card-section>
        <h2 class="text-h6 q-my-none">Contexto de acesso</h2>
        <p class="q-mt-sm">
          A unidade selecionada define os dados exibidos nesta área. As
          permissões são verificadas pela API em cada operação.
        </p>
        <q-chip outline color="primary">{{
          session.context?.isPlatformUser
            ? 'Identidade de plataforma'
            : 'Acesso do estabelecimento'
        }}</q-chip>
      </q-card-section></q-card
    >
  </q-page>
</template>
