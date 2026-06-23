import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

export type CustomerResponse = {
  id: string
  firstName: string
  lastName: string
  company?: string | null
  phoneNumber?: string | null
  email?: string | null
  street?: string | null
  houseNumber?: string | null
  postalCode?: string | null
  city?: string | null
  notes?: string | null
}

export async function getCustomers(): Promise<CustomerResponse[]> {
  const token = getAuthToken()

  if (!token) {
    throw new Error('Kein Authentifizierungs-Token vorhanden.')
  }

  const response = await fetch(`${API_BASE_URL}/api/customers`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    throw new Error('Kunden konnten nicht geladen werden.')
  }

  return response.json()
}