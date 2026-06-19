const AUTH_TOKEN_STORAGE_KEY = 'gartenzwerge.authToken'

export function saveAuthToken(token: string): void {
  localStorage.setItem(AUTH_TOKEN_STORAGE_KEY, token)
}

export function getAuthToken(): string | null {
  return localStorage.getItem(AUTH_TOKEN_STORAGE_KEY)
}

export function removeAuthToken(): void {
  localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY)
}

export function hasAuthToken(): boolean {
  return getAuthToken() !== null
}