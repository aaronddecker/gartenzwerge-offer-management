import { getAuthToken } from '../auth/authStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041'

/**
 * Thrown when the backend actively rejects the current token (HTTP 401).
 * Only this error should end the session — transient network failures or
 * requests aborted by a page reload must not log the user out.
 */
export class UnauthorizedError extends Error {
  constructor(message = 'Die aktuelle Sitzung ist ungültig oder abgelaufen.') {
    super(message)
    this.name = 'UnauthorizedError'
  }
}

type LoginRequest = {
  email: string
  password: string
}

type LoginResponse = {
  token: string
}

export type CurrentUserResponse = {
  userId: string
  email: string
  displayName?: string
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
    throw new UnauthorizedError('Kein Authentifizierungs-Token vorhanden.')
  }

  const response = await fetch(`${API_BASE_URL}/api/auth/me`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })

  if (response.status === 401 || response.status === 403) {
    throw new UnauthorizedError()
  }

  if (!response.ok) {
    throw new Error('Der aktuelle Benutzer konnte nicht geladen werden.')
  }

  return response.json()
}