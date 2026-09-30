<script setup lang="ts">
import { computed } from 'vue'
import { useSessionStore } from '../session/store'
const session = useSessionStore()
const unit = computed(() =>
  session.units.find((item) => item.id === session.unitId)
)
const shortcuts = computed(() => [
  { label: 'Catálogo', to: '/administration/catalog', available: session.can('management') },
  { label: 'Disponibilidade e horários', to: '/administration/availability', available: session.can('administration') },
  { label: 'Clientes', to: '/administration/customers', available: session.can('customer-operations') },
  { label: 'Regiões de entrega', to: '/administration/delivery-regions', available: session.can('management') },
  { label: 'Cupons', to: '/administration/coupons', available: session.can('promotion-management') },
  { label: 'Formas de pagamento', to: '/administration/payment-methods', available: session.can('payment-management') }
].filter((shortcut) => shortcut.available))
</script>
<template>
  <q-page class="admin-page">
    <header class="admin-page-header">
      <div>
        <h1 class="text-h4 q-my-none">Visão geral</h1>
        <p>{{ unit ? `Gerencie ${unit.name} em um só lugar.` : 'Nenhuma unidade autorizada disponível.' }}</p>
      </div>
    </header>
    <q-banner v-if="!unit" class="bg-amber-1 rounded-borders"
      >Solicite uma associação de unidade ao responsável pelo seu
      acesso.</q-banner
    >
    <template v-else>
      <section class="admin-home-context" aria-labelledby="active-unit-heading">
        <div>
          <p class="admin-home-context__label">UNIDADE ATIVA</p>
          <h2 id="active-unit-heading">{{ unit.name }}</h2>
          <p>A unidade selecionada define os dados exibidos nesta área. Cada operação continua protegida pelas permissões da sua conta.</p>
        </div>
        <q-chip outline color="primary">
          {{ session.context?.isPlatformUser ? 'Identidade de plataforma' : 'Acesso do estabelecimento' }}
        </q-chip>
      </section>

      <section v-if="shortcuts.length" class="admin-home-shortcuts" aria-labelledby="shortcuts-heading">
        <h2 id="shortcuts-heading" class="text-h6">Acessos frequentes</h2>
        <div class="admin-home-shortcuts__list">
          <q-btn
            v-for="shortcut in shortcuts"
            :key="shortcut.to"
            class="admin-home-shortcut"
            :to="shortcut.to"
            :label="shortcut.label"
            no-caps
            outline
            color="primary"
          />
        </div>
      </section>
    </template>
  </q-page>
</template>

<style scoped>
.admin-home-context {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 24px;
  padding: 24px;
  border: 1px solid var(--oh-border-subtle);
  border-radius: 16px;
  background: color-mix(in srgb, var(--oh-brand-soft) 58%, var(--oh-surface-raised));
}
.admin-home-context h2 { margin: 0; font-size: 1.5rem; }
.admin-home-context p { max-width: 68ch; margin: 8px 0 0; color: var(--oh-text-muted); }
.admin-home-context .admin-home-context__label { margin: 0 0 6px; color: var(--oh-brand-primary-text); font-size: .75rem; font-weight: 800; letter-spacing: .08em; }
.admin-home-shortcuts { margin-top: 32px; }
.admin-home-shortcuts h2 { margin-bottom: 14px; }
.admin-home-shortcuts__list { display: grid; grid-template-columns: repeat(auto-fit, minmax(min(100%, 230px), 1fr)); gap: 12px; }
.admin-home-shortcut { min-height: 56px; justify-content: flex-start; }
.admin-home-shortcut :deep(.q-btn__content) { justify-content: flex-start; width: 100%; }
@media (max-width: 600px) {
  .admin-home-context { align-items: flex-start; flex-direction: column; padding: 18px; }
}
</style>
