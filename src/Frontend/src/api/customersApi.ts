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

export type CreateCustomerRequest = {
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

function getAuthorizationHeader() {
  const token = getAuthToken()

  if (!token) {
    throw new Error('Kein Authentifizierungs-Token vorhanden.')
  }

  return {
    Authorization: `Bearer ${token}`,
  }
}

export async function getCustomers(): Promise<CustomerResponse[]> {
  const response = await fetch(`${API_BASE_URL}/api/customers`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Kunden konnten nicht geladen werden.')
  }

  return response.json()
}

export async function createCustomer(
  request: CreateCustomerRequest
): Promise<CustomerResponse> {
  const response = await fetch(`${API_BASE_URL}/api/customers`, {
    method: 'POST',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Kunde konnte nicht angelegt werden.')
  }

  return response.json()
}