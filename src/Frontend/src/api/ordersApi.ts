import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

function getAuthorizationHeader() {
  const token = getAuthToken()

  if (!token) {
    throw new Error('Kein Authentifizierungs-Token vorhanden.')
  }

  return {
    Authorization: `Bearer ${token}`,
  }
}

export type OrderStatus = 1 | 2 | 3 | 4

export type OrderResponse = {
  id: string
  offerId: string
  customerId: string
  status: OrderStatus
  plannedDate: string | null
  completedAt: string | null
  notes: string | null
}

export async function getOrders(): Promise<OrderResponse[]> {
  const response = await fetch(`${API_BASE_URL}/api/orders`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Aufträge konnten nicht geladen werden.')
  }

  return response.json()
}

export async function getOrderById(orderId: string): Promise<OrderResponse> {
  const response = await fetch(`${API_BASE_URL}/api/orders/${orderId}`, {
    headers: {
      ...getAuthorizationHeader(),
    },
  })

  if (!response.ok) {
    throw new Error('Auftrag konnte nicht geladen werden.')
  }

  return response.json()
}

export type UpdateOrderRequest = {
  status: OrderStatus
  plannedDate: string | null
  notes: string | null
}

export async function updateOrder(
  orderId: string,
  request: UpdateOrderRequest
): Promise<OrderResponse> {
  const response = await fetch(`${API_BASE_URL}/api/orders/${orderId}`, {
    method: 'PUT',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    const responseText = await response.text()

    throw new Error(
      responseText ||
        `Auftrag konnte nicht gespeichert werden. Statuscode: ${response.status}`
    )
  }

  return response.json()
}

export async function createOrderFromOffer(
  offerId: string
): Promise<OrderResponse> {
  const response = await fetch(`${API_BASE_URL}/api/offers/${offerId}/order`, {
    method: 'POST',
    headers: {
      ...getAuthorizationHeader(),
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({}),
  })

  if (!response.ok) {
    const responseText = await response.text()

    throw new Error(
      responseText ||
        `Auftrag konnte nicht erstellt werden. Statuscode: ${response.status}`
    )
  }

  return response.json()
}