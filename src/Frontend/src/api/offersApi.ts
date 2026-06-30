import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

export type OfferStatus = 1 | 2 | 3 | 4

export type OfferResponse = {
  id: string
  offerNumber: string
  customerId: string
  customerName: string
  createdAt: string
  validUntil: string
  status: OfferStatus
  totalNet: number
  notes?: string | null
}

export type CreateOfferRequest = {
  customerId: string
  validUntil: string
  notes?: string | null
}

export type UpdateOfferRequest = {
  validUntil: string
  status: OfferStatus
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

export async function getOffers(): Promise<OfferResponse[]> {
  const response = await fetch(`${API_BASE_URL}/api/offers`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Angebote konnten nicht geladen werden.')
  }

  return response.json()
}

export async function createOffer(
  request: CreateOfferRequest
): Promise<OfferResponse> {
  const response = await fetch(`${API_BASE_URL}/api/offers`, {
    method: 'POST',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Angebot konnte nicht angelegt werden.')
  }

  return response.json()
}

export async function getOfferById(offerId: string): Promise<OfferResponse> {
  const response = await fetch(`${API_BASE_URL}/api/offers/${offerId}`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Angebot konnte nicht geladen werden.')
  }

  return response.json()
}

export async function updateOffer(
  offerId: string,
  request: UpdateOfferRequest
): Promise<OfferResponse> {
  const response = await fetch(`${API_BASE_URL}/api/offers/${offerId}`, {
    method: 'PUT',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Angebot konnte nicht aktualisiert werden.')
  }

  return response.json()
}