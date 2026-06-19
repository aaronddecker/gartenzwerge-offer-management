import { useState } from 'react'

type LoginFormState = {
  email: string
  password: string
}

export function LoginPage() {
  const [formData, setFormData] = useState<LoginFormState>({
    email: '',
    password: '',
  })

  const [submitMessage, setSubmitMessage] = useState<string | null>(null)

function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
  event.preventDefault()

  setSubmitMessage('Login-Formular funktioniert. Der echte API-Login kommt im nächsten Schritt.')
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

          <button type="submit" className="primary-button">
            Einloggen
          </button>
          {submitMessage && <p className="form-message">{submitMessage}</p>}
        </form>
      </section>
    </main>
  )
}