<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import { isCancel } from 'axios'
import ProblemBanner from '../../../components/ProblemBanner.vue'
import { useSessionStore } from '../../session/store'
import {
  communicationsClient,
  type NotificationAttempt,
  type NotificationChannel,
  type NotificationConsent,
  type NotificationHistory,
  type NotificationTemplate,
  type NotificationTemplateInput
} from './client'

const session = useSessionStore()
const templates = ref<NotificationTemplate[]>([])
const consents = ref<NotificationConsent[]>([])
const history = ref<NotificationHistory[]>([])
const attemptHistory = ref<NotificationAttempt[]>([])
const attemptDialog = ref(false)
const attemptLoading = ref(false)
const selectedTemplateId = ref('')
const loading = ref(false)
const busy = ref(false)
const error = ref<unknown>(null)
const message = ref('')
const channelOptions: NotificationChannel[] = ['Email', 'WhatsApp']
const historyColumns = [
  { name: 'createdAtUtc', label: 'Data', field: 'createdAtUtc', align: 'left' as const, format: (value: string) => new Date(value).toLocaleString() },
  { name: 'channel', label: 'Canal', field: 'channel', align: 'left' as const },
  { name: 'purpose', label: 'Finalidade', field: 'purpose', align: 'left' as const },
  { name: 'destination', label: 'Destino', field: 'destination', align: 'left' as const },
  { name: 'status', label: 'Estado', field: 'status', align: 'left' as const, format: (value: string) => translateStatus(value) },
  { name: 'attemptCount', label: 'Tentativas', field: 'attemptCount', align: 'right' as const },
  { name: 'lastError', label: 'Código de erro', field: 'lastError', align: 'left' as const },
  { name: 'details', label: 'Histórico', field: 'id', align: 'left' as const }
]
const template = ref<NotificationTemplateInput>(blankTemplate())
const consent = ref({
  channel: 'Email' as NotificationChannel,
  purpose: 'order.update',
  destination: '',
  isGranted: true,
  source: ''
})
const send = ref({ destination: '', parametersJson: '{}', idempotencyKey: crypto.randomUUID() })
const selectedTemplate = computed(
  () => templates.value.find((item) => item.id === selectedTemplateId.value) ?? null
)
let request: AbortController | undefined

function blankTemplate(): NotificationTemplateInput {
  return {
    id: null,
    purpose: 'order.update',
    channel: 'Email',
    language: 'pt_BR',
    subject: '',
    body: '',
    providerTemplateName: null,
    requiresConsent: true,
    isActive: true
  }
}

function translateStatus(status: string) {
  return ({
    Queued: 'Na fila',
    AcceptedByProvider: 'Aceito pelo provedor',
    BlockedByConsent: 'Bloqueado por consentimento',
    RetryScheduled: 'Nova tentativa agendada',
    Failed: 'Falhou',
    Uncertain: 'Resultado incerto'
  } as Record<string, string>)[status] ?? status
}

async function load() {
  if (!session.unitId) return
  request?.abort()
  const current = new AbortController()
  request = current
  loading.value = true
  error.value = null
  try {
    const [loadedTemplates, loadedConsents, loadedHistory] = await Promise.all([
      communicationsClient.templates(session.unitId, current.signal),
      communicationsClient.consents(session.unitId, current.signal),
      communicationsClient.history(session.unitId, current.signal)
    ])
    if (current.signal.aborted) return
    templates.value = loadedTemplates
    consents.value = loadedConsents
    history.value = loadedHistory
    if (!loadedTemplates.some((item) => item.id === selectedTemplateId.value))
      selectedTemplateId.value = loadedTemplates[0]?.id ?? ''
  } catch (failure) {
    if (!isCancel(failure)) error.value = failure
  } finally {
    if (request === current) loading.value = false
  }
}

function edit(row: NotificationTemplate) {
  template.value = { ...row }
  selectedTemplateId.value = row.id
  message.value = ''
}

function create() {
  template.value = blankTemplate()
  message.value = ''
}

async function saveTemplate() {
  if (busy.value || !session.unitId) return
  busy.value = true
  error.value = null
  try {
    const result = await communicationsClient.saveTemplate(session.unitId, template.value)
    message.value = 'Template salvo.'
    await load()
    selectedTemplateId.value = result.id
    const stored = templates.value.find((item) => item.id === result.id)
    if (stored) edit(stored)
  } catch (failure) {
    error.value = failure
  } finally {
    busy.value = false
  }
}

async function saveConsent() {
  if (busy.value || !session.unitId) return
  busy.value = true
  error.value = null
  try {
    await communicationsClient.saveConsent(session.unitId, consent.value)
    message.value = 'Consentimento registrado. Informe uma origem verificável.'
    consents.value = await communicationsClient.consents(session.unitId)
  } catch (failure) {
    error.value = failure
  } finally {
    busy.value = false
  }
}

async function requestNotification() {
  if (busy.value || !session.unitId || !selectedTemplate.value) return
  busy.value = true
  error.value = null
  try {
    const parameters = JSON.parse(send.value.parametersJson) as Record<string, string>
    const result = await communicationsClient.request(session.unitId, {
      templateId: selectedTemplate.value.id,
      channel: selectedTemplate.value.channel,
      destination: send.value.destination,
      parameters,
      idempotencyKey: send.value.idempotencyKey
    })
    message.value = `Solicitação enfileirada (${result.id}).`
    send.value.idempotencyKey = crypto.randomUUID()
    history.value = await communicationsClient.history(session.unitId)
  } catch (failure) {
    error.value = failure
  } finally {
    busy.value = false
  }
}

async function showAttempts(notificationId: string) {
  if (!session.unitId) return
  attemptDialog.value = true
  attemptLoading.value = true
  try {
    attemptHistory.value = await communicationsClient.attempts(session.unitId, notificationId)
  } catch (failure) {
    error.value = failure
    attemptDialog.value = false
  } finally {
    attemptLoading.value = false
  }
}

watch(() => session.unitId, () => void load())
watch(() => template.value.channel, (channel) => {
  if (channel === 'WhatsApp') template.value.subject = ''
})
onMounted(() => void load())
onUnmounted(() => request?.abort())
</script>

<template>
  <q-page class="admin-page">
    <header class="admin-page-header">
      <div>
        <p class="admin-page-eyebrow">COMUNICAÇÕES</p>
        <h1 class="text-h4 q-my-sm">Notificações</h1>
        <p>Configure templates, consentimentos e acompanhe envios por e-mail e WhatsApp.</p>
      </div>
      <q-btn color="primary" icon="add" label="Novo template" :disable="!session.unitId" @click="create" />
    </header>
    <q-banner v-if="!session.unitId" class="bg-amber-1">Selecione uma unidade autorizada.</q-banner>
    <q-banner v-else class="bg-blue-1 q-mb-md">
      O estado “Aceito pelo provedor” confirma o recebimento da solicitação pelo SMTP ou pela Meta, não a entrega final.
    </q-banner>
    <p v-if="message" role="status" class="text-positive">{{ message }}</p>
    <ProblemBanner :error="error"><q-btn flat label="Tentar novamente" @click="load" /></ProblemBanner>

    <div class="row q-col-gutter-lg">
      <section class="col-12 col-lg-5" aria-labelledby="template-heading">
        <h2 id="template-heading" class="text-h6">Templates da unidade</h2>
        <q-list v-if="templates.length" bordered separator class="rounded-borders q-mb-lg">
          <q-item v-for="row in templates" :key="row.id" clickable :active="row.id === selectedTemplateId" @click="edit(row)">
            <q-item-section>
              <q-item-label>{{ row.purpose }} · {{ row.channel }}</q-item-label>
              <q-item-label caption>{{ row.language }} · {{ row.isActive ? 'Ativo' : 'Inativo' }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
        <p v-else-if="!loading" class="text-grey-7">Ainda não há templates para esta unidade.</p>

        <q-form class="q-gutter-md" @submit.prevent="saveTemplate">
          <q-input v-model="template.purpose" outlined label="Finalidade *" maxlength="100" required hint="Ex.: order.update" />
          <q-select v-model="template.channel" outlined label="Canal *" :options="channelOptions" />
          <q-input v-model="template.language" outlined label="Idioma *" maxlength="16" required hint="Ex.: pt_BR" />
          <q-input v-if="template.channel === 'Email'" v-model="template.subject" outlined label="Assunto do e-mail *" maxlength="250" required />
          <q-input v-if="template.channel === 'WhatsApp'" v-model="template.providerTemplateName" outlined label="Nome do template aprovado na Meta *" maxlength="200" required />
          <q-input
            v-model="template.body"
            outlined
            type="textarea"
            label="Corpo / referência do template *"
            maxlength="10000"
            required
            :hint="template.channel === 'Email' ? 'Use {{name}} para parâmetros do e-mail.' : 'Use {{1}}, {{2}} para os parâmetros do template aprovado na Meta.'"
          />
          <q-toggle v-model="template.requiresConsent" label="Exigir consentimento antes do envio" />
          <q-toggle v-model="template.isActive" label="Template ativo" />
          <q-btn type="submit" color="primary" label="Salvar template" :loading="busy" />
        </q-form>
      </section>

      <section class="col-12 col-lg-7" aria-labelledby="consent-heading">
        <h2 id="consent-heading" class="text-h6">Consentimento e envio</h2>
        <p class="text-body2">Registre apenas consentimento comprovado. Sem consentimento para finalidade exigida, o gateway não tenta enviar.</p>
        <q-form class="row q-col-gutter-sm items-start q-mb-lg" @submit.prevent="saveConsent">
          <q-select v-model="consent.channel" class="col-12 col-sm-4" outlined label="Canal" :options="channelOptions" />
          <q-input v-model="consent.purpose" class="col-12 col-sm-4" outlined label="Finalidade" maxlength="100" />
          <q-input v-model="consent.destination" class="col-12 col-sm-4" outlined label="E-mail ou telefone E.164" maxlength="254" />
          <q-input v-model="consent.source" class="col-12 col-sm-8" outlined label="Origem verificável do consentimento *" maxlength="200" required />
          <q-toggle v-model="consent.isGranted" class="col-12 col-sm-4" label="Consentimento concedido" />
          <div class="col-12"><q-btn type="submit" outline label="Registrar consentimento" :loading="busy" /></div>
        </q-form>

        <q-separator class="q-my-lg" />
        <h3 class="text-subtitle1">Solicitar notificação</h3>
        <q-form class="q-gutter-sm q-mb-lg" @submit.prevent="requestNotification">
          <q-select v-model="selectedTemplateId" outlined label="Template ativo *" emit-value map-options
            :options="templates.filter((item) => item.isActive).map((item) => ({ label: `${item.purpose} · ${item.channel} · ${item.language}`, value: item.id }))" />
          <q-input v-model="send.destination" outlined label="Destino *" maxlength="254" required />
          <q-input v-model="send.parametersJson" outlined type="textarea" label="Parâmetros JSON" hint='Ex.: {"name":"Ana"} ou {"1":"Ana"} para WhatsApp.' />
          <q-input v-model="send.idempotencyKey" outlined label="Chave idempotente *" maxlength="200" required />
          <q-btn type="submit" outline label="Enfileirar envio" :disable="!selectedTemplate" :loading="busy" />
        </q-form>

        <h3 class="text-subtitle1">Consentimentos registrados</h3>
        <q-list bordered separator class="rounded-borders q-mb-lg">
          <q-item v-for="item in consents" :key="item.id">
            <q-item-section>
              <q-item-label>{{ item.destination }} · {{ item.channel }} · {{ item.purpose }}</q-item-label>
              <q-item-label caption>{{ item.isGranted ? 'Concedido' : 'Revogado' }} · {{ item.source || 'Origem não informada' }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item v-if="!consents.length"><q-item-section class="text-grey-7">Nenhum consentimento registrado.</q-item-section></q-item>
        </q-list>
      </section>

      <section class="col-12" aria-labelledby="history-heading">
        <h2 id="history-heading" class="text-h6">Histórico recente</h2>
        <q-table
          :rows="history"
          row-key="id"
          :loading="loading"
          :pagination="{ rowsPerPage: 10 }"
          :columns="historyColumns"
          no-data-label="Ainda não há notificações para esta unidade."
        >
          <template #body-cell-details="props">
            <q-td :props="props">
              <q-btn flat dense label="Ver tentativas" :disable="!props.row.attemptCount" @click="showAttempts(props.row.id)" />
            </q-td>
          </template>
        </q-table>
      </section>
    </div>

    <q-dialog v-model="attemptDialog" aria-labelledby="attempt-dialog-title">
      <q-card style="min-width: min(36rem, 92vw)">
        <q-card-section class="row items-center">
          <h2 id="attempt-dialog-title" class="text-h6 q-my-none">Tentativas de envio</h2>
          <q-space />
          <q-btn flat round dense icon="close" aria-label="Fechar histórico" v-close-popup />
        </q-card-section>
        <q-card-section>
          <q-linear-progress v-if="attemptLoading" indeterminate />
          <q-list v-else bordered separator>
            <q-item v-for="attempt in attemptHistory" :key="attempt.attemptNumber">
              <q-item-section>
                <q-item-label>Tentativa {{ attempt.attemptNumber }} · {{ translateStatus(attempt.status) }}</q-item-label>
                <q-item-label caption>{{ new Date(attempt.createdAtUtc).toLocaleString() }} · {{ attempt.safeErrorCode || attempt.providerMessageId || 'Sem código de erro' }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item v-if="!attemptHistory.length"><q-item-section>Nenhuma tentativa registrada.</q-item-section></q-item>
          </q-list>
        </q-card-section>
        <q-card-actions align="right"><q-btn flat label="Fechar" v-close-popup /></q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
