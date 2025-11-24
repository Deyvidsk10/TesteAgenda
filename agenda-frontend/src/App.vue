<template>
  <div class="page-root">
    <div class="page-container">
      <Toast />

      <div class="card">
        <!-- HEADER -->
        <header class="header">
          <div>
            <h1 class="title">Agenda de Contatos</h1>
            <p class="subtitle">Gerencie seus contatos de forma simples e rápida.</p>
          </div>

          <div class="header-info">
            <div class="contacts-count">{{ contacts.length }} contato(s)</div>
            <div v-if="loadingContacts" class="loading-text">Carregando contatos...</div>
          </div>
        </header>

        <div class="content-grid">
          <!-- FORMULÁRIO -->
          <section class="form-section">
            <h2 class="section-title">
              {{ editingId ? 'Editar contato' : 'Novo contato' }}
            </h2>

            <div class="form-grid">
              <div class="form-field">
                <label for="name">Nome</label>
                <InputText
                  id="name"
                  v-model="form.name"
                  placeholder="Digite o nome"
                />
              </div>

              <div class="form-field">
                <label for="email">E-mail</label>
                <InputText
                  id="email"
                  v-model="form.email"
                  placeholder="Digite o e-mail"
                />
              </div>

              <div class="form-field">
                <label for="phone">Telefone</label>
                <InputText
                  id="phone"
                  :value="form.phone"
                  @input="onPhoneInput"
                  placeholder="(81) 99999-0000"
                />
              </div>
            </div>

            <div class="buttons-row">
              <Button
                :label="editingId ? 'Salvar alterações' : 'Adicionar'"
                icon="pi pi-check"
                :loading="loading"
                @click="onSubmit"
              />
              <Button
                v-if="editingId"
                label="Cancelar edição"
                class="p-button-secondary"
                icon="pi pi-times"
                :disabled="loading"
                @click="resetForm"
              />
            </div>
          </section>

          <!-- LISTA / TABELA -->
          <section class="table-section">
            <div class="table-header">
              <h2 class="section-title">Contatos</h2>

              <div class="search-wrapper">
                <span class="pi pi-search search-icon"></span>
                <InputText
                  v-model="filter"
                  placeholder="Buscar por nome, e-mail ou telefone"
                  class="search-input"
                />
              </div>
            </div>

            <DataTable
              :value="filteredContacts"
              dataKey="id"
              stripedRows
              responsiveLayout="scroll"
              :loading="loadingContacts"
              emptyMessage="Nenhum contato cadastrado."
            >
              <Column field="name" header="Nome" />
              <Column field="email" header="E-mail" />
              <Column field="phone" header="Telefone" />
              <Column header="Ações" style="width: 180px">
                <template #body="slotProps">
                  <div class="actions-row">
                    <Button
                      icon="pi pi-pencil"
                      class="p-button-sm p-button-warning"
                      @click="onEdit(slotProps.data)"
                    />
                    <Button
                      icon="pi pi-trash"
                      class="p-button-sm p-button-danger"
                      @click="onDelete(slotProps.data)"
                    />
                  </div>
                </template>
              </Column>
            </DataTable>
          </section>
        </div>
      </div>
    </div>
  </div>
</template>


<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

// PrimeVue
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'

const toast = useToast()

// URL base da API
const api = axios.create({
  baseURL: 'https://localhost:52203/api/contacts'
})


const contacts = ref([])


const loading = ref(false)
const loadingContacts = ref(false)


const globalFilter = ref('')

const form = ref({
  name: '',
  email: '',
  phone: ''
})


const errors = ref({
  name: '',
  email: '',
  phone: ''
})

const editingId = ref(null)

const filter = ref('')

const filteredContacts = computed(() => {
  if (!filter.value) return contacts.value

  const term = filter.value.toLowerCase()
  return contacts.value.filter((c) =>
    (c.name && c.name.toLowerCase().includes(term)) ||
    (c.email && c.email.toLowerCase().includes(term)) ||
    (c.phone && c.phone.toLowerCase().includes(term))
  )
})


// Carregar contatos
const loadContacts = async () => {
  try {
    loadingContacts.value = true
    const { data } = await api.get('/')
    contacts.value = data
  } catch (err) {
    console.error(err)
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: 'Não foi possível carregar os contatos.',
      life: 3000
    })
  } finally {
    loadingContacts.value = false
  }
}

// Resetar erros
const resetErrors = () => {
  errors.value = {
    name: '',
    email: '',
    phone: ''
  }
}

// Validação simples
const NAME_MIN = 3
const NAME_MAX = 100
const EMAIL_MAX = 150
const PHONE_MIN = 10
const PHONE_MAX = 11

const validateForm = () => {
  resetErrors()
  let valid = true

  // Nome
  if (!form.value.name) {
    errors.value.name = 'Nome é obrigatório.'
    valid = false
  } else if (form.value.name.length < NAME_MIN) {
    errors.value.name = `Nome deve ter pelo menos ${NAME_MIN} caracteres.`
    valid = false
  } else if (form.value.name.length > NAME_MAX) {
    errors.value.name = `Nome deve ter no máximo ${NAME_MAX} caracteres.`
    valid = false
  }

  // E-mail
  if (!form.value.email) {
    errors.value.email = 'E-mail é obrigatório.'
    valid = false
  } else if (form.value.email.length > EMAIL_MAX) {
    errors.value.email = `E-mail deve ter no máximo ${EMAIL_MAX} caracteres.`
    valid = false
  } else if (!/\S+@\S+\.\S+/.test(form.value.email)) {
    errors.value.email = 'E-mail inválido.'
    valid = false
  }

  // Telefone
  if (!form.value.phone) {
    errors.value.phone = 'Telefone é obrigatório.'
    valid = false
  } else {
    const digits = form.value.phone.replace(/\D/g, '')

    if (digits.length < PHONE_MIN || digits.length > PHONE_MAX) {
      errors.value.phone = `Telefone deve ter entre ${PHONE_MIN} e ${PHONE_MAX} dígitos.`
      valid = false
    }
  }

  return valid
}


// Resetar formulário
const resetForm = () => {
  form.value = {
    name: '',
    email: '',
    phone: ''
  }
  editingId.value = null
  resetErrors()
}

// Máscara de telefone
const onPhoneInput = (event) => {
  let value = event.target.value.replace(/\D/g, '')

  if (value.length > 11) value = value.slice(0, 11)

  if (value.length <= 10) {
    // (81) 9999-9999
    value = value.replace(/(\d{2})(\d)/, '($1) $2')
    value = value.replace(/(\d{4})(\d)/, '$1-$2')
  } else {
    // (81) 99999-9999
    value = value.replace(/(\d{2})(\d)/, '($1) $2')
    value = value.replace(/(\d{5})(\d)/, '$1-$2')
  }

  form.value.phone = value
}

// Cadastrar / Editar
const onSubmit = async () => {
  try {
    if (!validateForm()) {
      toast.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Verifique os campos obrigatórios.',
        life: 3000
      })
      return
    }

    loading.value = true

    if (editingId.value) {
      await api.put(`/${editingId.value}`, form.value)
      toast.add({
        severity: 'success',
        summary: 'Contato atualizado',
        detail: 'Os dados do contato foram salvos com sucesso.',
        life: 3000
      })
    } else {
      await api.post('/', form.value)
      toast.add({
        severity: 'success',
        summary: 'Contato criado',
        detail: 'Contato adicionado à agenda.',
        life: 3000
      })
    }

    resetForm()
    await loadContacts()
  } catch (err) {
    console.error(err)
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: 'Erro ao salvar o contato.',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}


// Editar 
const onEdit = (contact) => {
  editingId.value = contact.id
  form.value = {
    name: contact.name,
    email: contact.email,
    phone: contact.phone
  }
  resetErrors()
}

// Remover
const onDelete = async (contact) => {
  if (!confirm(`Deseja realmente excluir o contato "${contact.name}"?`)) {
    return
  }

  try {
    await api.delete(`/${contact.id}`)
    toast.add({
      severity: 'success',
      summary: 'Contato removido',
      detail: 'O contato foi excluído.',
      life: 3000
    })
    await loadContacts()
  } catch (err) {
    console.error(err)
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: 'Erro ao excluir o contato.',
      life: 3000
    })
  }
}

onMounted(() => {
  loadContacts()
})
</script>

<style>
.page-root {
  min-height: 100vh;
  background: #d1d2d4; 
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding: 2rem 1rem;
}

.page-container {
  width: 100%;
  max-width: 1100px; 
}

.card {
  background: #ffffff;
  border-radius: 10px;
  padding: 1.75rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
}


.header {
  display: flex;
  justify-content: space-between;
  gap: 1.5rem;
  align-items: flex-start;
  margin-bottom: 1.5rem;
}

.title {
  font-size: 1.8rem;
  margin: 0 0 0.25rem 0;
}

.subtitle {
  color: #6b7280;
  margin: 0;
}

.header-info {
  text-align: right;
  font-size: 0.9rem;
  color: #4b5563;
}

.contacts-count {
  font-weight: 600;
}

.loading-text {
  margin-top: 0.25rem;
  font-style: italic;
}

/* GRID PRINCIPAL */
.content-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
}

@media (min-width: 900px) {
  .content-grid {
    grid-template-columns: 0.9fr 1.1fr; 
  }
}

/* FORMULÁRIO */
.form-section {
  border-right: 1px solid #e5e7eb;
  padding-right: 1.5rem;
}

@media (max-width: 899px) {
  .form-section {
    border-right: none;
    padding-right: 0;
    border-bottom: 1px solid #e5e7eb;
    padding-bottom: 1.5rem;
  }
}

.section-title {
  font-size: 1.1rem;
  margin-bottom: 1rem;
}

.form-grid {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.form-field label {
  display: block;
  font-size: 0.9rem;
  margin-bottom: 0.25rem;
  color: #374151;
}

.buttons-row {
  margin-top: 1rem;
  display: flex;
  gap: 0.5rem;
}

/* TABELA */
.table-section {
  min-width: 0;
}

.table-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.search-wrapper {
  position: relative;
  width: 100%;
  max-width: 260px;
}

.search-icon {
  position: absolute;
  top: 50%;
  left: 0.6rem;
  transform: translateY(-50%);
  font-size: 0.9rem;
  color: #9ca3af;
  z-index: 1;
}

.search-input {
  width: 100%;
  padding-left: 2rem;
}

/* AÇÕES */
.actions-row {
  display: flex;
  gap: 0.4rem;
  justify-content: center;
}

</style>
