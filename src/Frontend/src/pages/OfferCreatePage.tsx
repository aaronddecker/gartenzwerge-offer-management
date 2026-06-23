import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { getCustomers, type CustomerResponse } from '../api/customersApi'
import {
  createOffer,
  type CreateOfferRequest,
} from '../api/offersApi'
import { PageHeader } from '../shared/components/PageHeader'

type OfferCreateFormState = {
  customerId: string
  validUntil: string
  notes: string
}

const initialOfferCreateFormData: OfferCreateFormState = {
  customerId: '',
  validUntil: '',
  notes: '',
}

function emptyToNull(value: string) {
  const trimmedValue = value.trim()

  return trimmedValue.length > 0 ? trimmedValue : null
}

function toCreateOfferRequest(
  formData: OfferCreateFormState
): CreateOfferRequest {
  return {
    customerId: formData.customerId,
    validUntil: new Date(`${formData.validUntil}T00:00:00.000Z`).toISOString(),
    notes: emptyToNull(formData.notes),
  }
}

function getCustomerDisplayName(customer: CustomerResponse) {
  return customer.company || `${customer.firstName} ${customer.lastName}`
}

export function OfferCreatePage() {
  const navigate = useNavigate()

  const [customers, setCustomers] = useState<CustomerResponse[]>([])
  const [isLoadingCustomers, setIsLoadingCustomers] = useState(true)
  const [customerErrorMessage, setCustomerErrorMessage] = useState<
    string | null
  >(null)

  const [formData, setFormData] = useState<OfferCreateFormState>(
    initialOfferCreateFormData
  )
  const [isCreating, setIsCreating] = useState(false)
  const [createErrorMessage, setCreateErrorMessage] = useState<string | null>(
    null
  )

  useEffect(() => {
    let isMounted = true

    getCustomers()
      .then((loadedCustomers) => {
        if (!isMounted) {
          return
        }

        setCustomers(loadedCustomers)
        setCustomerErrorMessage(null)
      })
      .catch((error) => {
        if (!isMounted) {
          return
        }

        setCustomerErrorMessage(
          error instanceof Error
            ? error.message
            : 'Ein unbekannter Fehler ist aufgetreten.'
        )
      })
      .finally(() => {
        if (!isMounted) {
          return
        }

        setIsLoadingCustomers(false)
      })

    return () => {
      isMounted = false
    }
  }, [])

  function handleFormChange(field: keyof OfferCreateFormState, value: string) {
    setFormData({
      ...formData,
      [field]: value,
    })
  }

  async function handleCreateOffer(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    setIsCreating(true)
    setCreateErrorMessage(null)

    try {
      await createOffer(toCreateOfferRequest(formData))
      navigate('/offers')
    } catch (error) {
      setCreateErrorMessage(
        error instanceof Error
          ? error.message
          : 'Ein unbekannter Fehler ist aufgetreten.'
      )
    } finally {
      setIsCreating(false)
    }
  }

  return (
    <section className="page">
      <PageHeader
        title="Neues Angebot"
        description="Erstelle einen Angebotskopf für einen bestehenden Kunden."
      />

      <div className="page-actions page-actions--left">
        <Link to="/offers" className="secondary-link-button">
          Zurück zu Angeboten
        </Link>
      </div>

      <section className="offer-form-card">
        <h3>Angebotsdaten</h3>

        {isLoadingCustomers && (
          <p className="muted-text">Kunden werden geladen...</p>
        )}

        {!isLoadingCustomers && customerErrorMessage && (
          <p className="form-message form-message--error">
            {customerErrorMessage}
          </p>
        )}

        {!isLoadingCustomers && !customerErrorMessage && customers.length === 0 && (
          <p className="muted-text">
            Es sind noch keine Kunden vorhanden. Lege zuerst einen Kunden an.
          </p>
        )}

        {!isLoadingCustomers && !customerErrorMessage && customers.length > 0 && (
          <form className="offer-form" onSubmit={handleCreateOffer}>
            <div className="offer-form-grid">
              <label className="form-field">
                <span>Kunde</span>
                <select
                  value={formData.customerId}
                  onChange={(event) =>
                    handleFormChange('customerId', event.target.value)
                  }
                  required
                >
                  <option value="">Kunde auswählen</option>

                  {customers.map((customer) => (
                    <option key={customer.id} value={customer.id}>
                      {getCustomerDisplayName(customer)}
                    </option>
                  ))}
                </select>
              </label>

              <label className="form-field">
                <span>Gültig bis</span>
                <input
                  type="date"
                  value={formData.validUntil}
                  onChange={(event) =>
                    handleFormChange('validUntil', event.target.value)
                  }
                  required
                />
              </label>

              <label className="form-field offer-form__full">
                <span>Notizen</span>
                <textarea
                  value={formData.notes}
                  onChange={(event) =>
                    handleFormChange('notes', event.target.value)
                  }
                />
              </label>
            </div>

            <button
              type="submit"
              className="primary-button"
              disabled={isCreating}
            >
              {isCreating ? 'Angebot wird gespeichert...' : 'Angebot anlegen'}
            </button>

            {createErrorMessage && (
              <p className="form-message form-message--error">
                {createErrorMessage}
              </p>
            )}
          </form>
        )}
      </section>
    </section>
  )
}
