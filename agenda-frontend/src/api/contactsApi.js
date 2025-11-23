import axios from 'axios'


const api = axios.create({
  baseURL: 'http://localhost:52203'
})

export async function getContacts() {
  const response = await api.get('/api/contacts')
  return response.data
}

export async function createContact(payload) {
  const response = await api.post('/api/contacts', payload)
  return response.data
}

export async function updateContact(id, payload) {
  const response = await api.put(`/api/contacts/${id}`, payload)
  return response.data
}

export async function deleteContact(id) {
  await api.delete(`/api/contacts/${id}`)
}
