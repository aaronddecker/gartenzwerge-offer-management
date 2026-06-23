import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

export type OfferedServiceResponse = {
  id: string
  name: string
  description?: string | null
  unit: string
  basePrice: number
  isActive: boolean
}

export type CreateOfferedServiceRequest = {
  name: string
  description?: string | null
  unit: string
  basePrice: number
  isActive: boolean
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

export async function getOfferedServices(): Promise<OfferedServiceResponse[]> {
  const response = await fetch(`${API_BASE_URL}/api/offered-services`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Leistungen konnten nicht geladen werden.')
  }

  return response.json()
}

export async function createOfferedService(
  request: CreateOfferedServiceRequest
): Promise<OfferedServiceResponse> {
  const response = await fetch(`${API_BASE_URL}/api/offered-services`, {
    method: 'POST',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Leistung konnte nicht angelegt werden.')
  }

  return response.json()
}
