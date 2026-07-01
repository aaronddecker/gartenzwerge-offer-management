import type { FormEvent } from 'react'

export type CustomerFormState = {
  firstName: string
  lastName: string
  company: string
  phoneNumber: string
  email: string
  street: string
  houseNumber: string
  postalCode: string
  city: string
  notes: string
}

type CustomerFormProps = {
  formData: CustomerFormState
  isEditing: boolean
  isSaving: boolean
  isInline?: boolean
  errorMessage: string | null
  successMessage: string | null
  onFieldChange: (field: keyof CustomerFormState, value: string) => void
  onSubmit: (event: FormEvent<HTMLFormElement>) => void
  onCancel: () => void
}

export function CustomerForm({
  formData,
  isEditing,
  isSaving,
  isInline = false,
  errorMessage,
  successMessage,
  onFieldChange,
  onSubmit,
  onCancel,
}: CustomerFormProps) {
  return (
    <section
      className={
        isInline
          ? 'customer-form-card customer-form-card--inline'
          : 'customer-form-card'
      }
    >
      <h3>{isEditing ? 'Kunden bearbeiten' : 'Neuen Kunden anlegen'}</h3>

      <form className="customer-form" onSubmit={onSubmit}>
        <div className="customer-form-grid">
          <label className="form-field">
            <span>Vorname</span>
            <input
              value={formData.firstName}
              onChange={(event) => onFieldChange('firstName', event.target.value)}
              required
            />
          </label>

          <label className="form-field">
            <span>Nachname</span>
            <input
              value={formData.lastName}
              onChange={(event) => onFieldChange('lastName', event.target.value)}
              required
            />
          </label>

          <label className="form-field">
            <span>Firma</span>
            <input
              value={formData.company}
              onChange={(event) => onFieldChange('company', event.target.value)}
            />
          </label>

          <label className="form-field">
            <span>E-Mail</span>
            <input
              type="email"
              value={formData.email}
              onChange={(event) => onFieldChange('email', event.target.value)}
            />
          </label>

          <label className="form-field">
            <span>Telefon</span>
            <input
              value={formData.phoneNumber}
              onChange={(event) =>
                onFieldChange('phoneNumber', event.target.value)
              }
            />
          </label>

          <label className="form-field">
            <span>Ort</span>
            <input
              value={formData.city}
              onChange={(event) => onFieldChange('city', event.target.value)}
            />
          </label>

          <label className="form-field">
            <span>Straße</span>
            <input
              value={formData.street}
              onChange={(event) => onFieldChange('street', event.target.value)}
            />
          </label>

          <label className="form-field">
            <span>Hausnummer</span>
            <input
              value={formData.houseNumber}
              onChange={(event) =>
                onFieldChange('houseNumber', event.target.value)
              }
            />
          </label>

          <label className="form-field">
            <span>Postleitzahl</span>
            <input
              value={formData.postalCode}
              onChange={(event) =>
                onFieldChange('postalCode', event.target.value)
              }
            />
          </label>

          <label className="form-field customer-form__full">
            <span>Notizen</span>
            <textarea
              value={formData.notes}
              onChange={(event) => onFieldChange('notes', event.target.value)}
            />
          </label>
        </div>

        <div className="customer-form-actions">
          <button
            type="button"
            className="secondary-button"
            onClick={onCancel}
          >
            Abbrechen
          </button>

          <button type="submit" className="primary-button" disabled={isSaving}>
            {isSaving
              ? 'Kunde wird gespeichert...'
              : isEditing
                ? 'Speichern'
                : 'Kunde anlegen'}
          </button>
        </div>

        {errorMessage && (
          <p className="form-message form-message--error">{errorMessage}</p>
        )}

        {successMessage && (
          <p className="form-message form-message--success">{successMessage}</p>
        )}
      </form>
    </section>
  )
}