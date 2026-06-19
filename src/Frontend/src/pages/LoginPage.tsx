import { useState } from 'react'
import { login } from '../api/authApi'
import { saveAuthToken } from '../auth/authStorage'

type LoginFormState = {
  email: string
  password: string
}

export function LoginPage() {
  const [formData, setFormData] = useState<LoginFormState>({
    email: '',
    password: '',
  })

  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()

    setIsSubmitting(true)
    setErrorMessage(null)
    setSuccessMessage(null)

   try {
  const response = await login(formData)

  saveAuthToken(response.token)

  setSuccessMessage('Login erfolgreich. Token wurde gespeichert.')
} catch (error) {
      setErrorMessage(
        error instanceof Error
          ? error.message
          : 'Ein unbekannter Fehler ist aufgetreten.'
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="login-page">
      <section className="login-card">
        <p className="app-kicker">Gartenzwerge Außenservice</p>
        <h1>Einloggen</h1>
        <p className="login-card__description">
          Melde dich an, um Kunden, Angebote und Aufträge zu verwalten.
        </p>

        <form className="login-form" onSubmit={handleSubmit}>
          <label className="form-field">
            <span>E-Mail</span>
            <input
              type="email"
              value={formData.email}
              onChange={(event) =>
                setFormData({
                  ...formData,
                  email: event.target.value,
                })
              }
              placeholder="test@gartenzwerge.de"
              autoComplete="email"
              required
            />
          </label>

          <label className="form-field">
            <span>Passwort</span>
            <input
              type="password"
              value={formData.password}
              onChange={(event) =>
                setFormData({
                  ...formData,
                  password: event.target.value,
                })
              }
              placeholder="Dein Passwort"
              autoComplete="current-password"
              required
            />
          </label>

          <button type="submit" className="primary-button" disabled={isSubmitting}>
            {isSubmitting ? 'Einloggen...' : 'Einloggen'}
          </button>

          {errorMessage && (
            <p className="form-message form-message--error">{errorMessage}</p>
          )}

          {successMessage && (
            <p className="form-message form-message--success">{successMessage}</p>
          )}
        </form>
      </section>
    </main>
  )
}