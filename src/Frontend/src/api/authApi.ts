import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

type LoginRequest = {
  email: string
  password: string
}

type LoginResponse = {
  token: string
}

export type CurrentUserResponse = {
  userId?: string
  id?: string
  email: string
  roles?: string[]
}

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error('Login fehlgeschlagen. Bitte prüfe E-Mail und Passwort.')
  }

  return response.json()
}

export async function getCurrentUser(): Promise<CurrentUserResponse> {
  const token = getAuthToken()

  if (!token) {
    throw new Error('Kein Authentifizierungs-Token vorhanden.')
  }

  const response = await fetch(`${API_BASE_URL}/api/auth/me`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })

  if (!response.ok) {
    throw new Error('Die aktuelle Sitzung ist ungültig oder abgelaufen.')
  }

  return response.json()
}