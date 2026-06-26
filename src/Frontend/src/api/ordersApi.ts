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

export async function createOrderFromOffer(offerId: string): Promise<void> {
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
}