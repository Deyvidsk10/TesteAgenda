<template>
  <div class="p-4">
    <Toast />

    <h1 class="text-2xl font-bold mb-4">Agenda de Contatos</h1>

    <!-- FORMULÁRIO -->
    <div class="card p-3 mb-4">
      <h2 class="text-xl font-semibold mb-3">
        {{ editingId ? 'Editar contato' : 'Novo contato' }}
      </h2>

      <div class="p-fluid grid formgrid">
        <div class="field col-12 md:col-4">
          <label for="name">Nome</label>
          <InputText
            id="name"
            v-model="form.name"
            placeholder="Digite o nome"
          />
        </div>

        <div class="field col-12 md:col-4">
          <label for="email">E-mail</label>
          <InputText
            id="email"
            v-model="form.email"
            placeholder="Digite o e-mail"
          />
        </div>

        <div class="field col-12 md:col-4">
          <label for="phone">Telefone</label>
          <InputText
            id="phone"
            v-model="form.phone"
            placeholder="Digite o telefone"
          />
        </div>
      </div>

      <div class="flex gap-2 mt-3">
        <Button
          :label="editingId ? 'Salvar alterações' : 'Adicionar'"
          icon="pi pi-check"
          @click="onSubmit"
        />
        <Button
          v-if="editingId"
          label="Cancelar edição"
          class="p-button-secondary"
          icon="pi pi-times"
          @click="resetForm"
        />
      </div>
    </div>

    <!-- TABELA -->
    <div class="card">
      <h2 class="text-xl font-semibold mb-3">Contatos</h2>

   <DataTable :value="contacts" dataKey="id" stripedRows responsiveLayout="scroll">
  <Column field="name" header="Nome" />
  <Column field="email" header="E-mail" />
  <Column field="phone" header="Telefone" />
  <Column header="Ações" style="width: 180px">
    <template #body="slotProps">
      <div class="flex gap-2">
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

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useToast } from 'primevue/usetoast'
import Toast from 'primevue/toast'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'

const toast = useToast()


const api = axios.create({
  baseURL: 'https://localhost:52203/api/contacts'
})



const contacts = ref([])


const form = ref({
  name: '',
  email: '',
  phone: ''
})

const editingId = ref(null)

// Carregar contatos
const loadContacts = async () => {
  try {
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
  }
}

// Resetar formulário
const resetForm = () => {
  form.value = {
    name: '',
    email: '',
    phone: ''
  }
  editingId.value = null
}

// Cadastrar / Editar
const onSubmit = async () => {
  try {
    if (!form.value.name || !form.value.email || !form.value.phone) {
      toast.add({
        severity: 'warn',
        summary: 'Atenção',
        detail: 'Preencha todos os campos.',
        life: 3000
      })
      return
    }

    if (editingId.value) {
      // UPDATE
      await api.put(`/${editingId.value}`, form.value)
      toast.add({
        severity: 'success',
        summary: 'Contato atualizado',
        detail: 'Os dados do contato foram salvos com sucesso.',
        life: 3000
      })
    } else {
      // CREATE
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
  }
}

// Editar (preenche o form)
const onEdit = (contact) => {
  editingId.value = contact.id
  form.value = {
    name: contact.name,
    email: contact.email,
    phone: contact.phone
  }
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
.card {
  background: #fff;
  border-radius: 6px;
  padding: 1rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}
</style>