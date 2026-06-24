import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

export type OfferItemResponse = {
  id: string
  offerId: string
  offeredServiceId: string
  description: string
  quantity: number
  unit: string
  unitPrice: number
  totalPrice: number
}

export type CreateOfferItemRequest = {
  offeredServiceId: string
  quantity: number
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

export async function getOfferItems(
  offerId: string
): Promise<OfferItemResponse[]> {
  const response = await fetch(`${API_BASE_URL}/api/offers/${offerId}/items`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Angebotspositionen konnten nicht geladen werden.')
  }

  return response.json()
}

export async function createOfferItem(
  offerId: string,
  request: CreateOfferItemRequest
): Promise<OfferItemResponse> {
  const response = await fetch(`${API_BASE_URL}/api/offers/${offerId}/items`, {
    method: 'POST',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Angebotsposition konnte nicht angelegt werden.')
  }

  return response.json()
}