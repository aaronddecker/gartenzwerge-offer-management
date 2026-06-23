import { useEffect, useState, type FormEvent } from 'react'
import { useOutletContext } from 'react-router-dom'
import type { CurrentUserResponse } from '../api/authApi'
import {
  createCustomer,
  deleteCustomer,
  getCustomers,
  type CreateCustomerRequest,
  type CustomerResponse,
} from '../api/customersApi'
import { PageHeader } from '../shared/components/PageHeader'

type CustomerFormState = {
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

type AppLayoutOutletContext = {
  currentUser: CurrentUserResponse | null
}

const initialCustomerFormData: CustomerFormState = {
  firstName: '',
  lastName: '',
  company: '',
  phoneNumber: '',
  email: '',
  street: '',
  houseNumber: '',
  postalCode: '',
  city: '',
  notes: '',
}

function hasRole(user: CurrentUserResponse | null, role: string) {
  return user?.roles?.includes(role) ?? false
}

function emptyToNull(value: string) {
  const trimmedValue = value.trim()

  return trimmedValue.length > 0 ? trimmedValue : null
}

function toCreateCustomerRequest(
  formData: CustomerFormState
): CreateCustomerRequest {
  return {
    firstName: formData.firstName.trim(),
    lastName: formData.lastName.trim(),
    company: emptyToNull(formData.company),
    phoneNumber: emptyToNull(formData.phoneNumber),
    email: emptyToNull(formData.email),
    street: emptyToNull(formData.street),
    houseNumber: emptyToNull(formData.houseNumber),
    postalCode: emptyToNull(formData.postalCode),
    city: emptyToNull(formData.city),
    notes: emptyToNull(formData.notes),
  }
}

export function CustomersPage() {
  const { currentUser } = useOutletContext<AppLayoutOutletContext>()
  const isAdmin = hasRole(currentUser, 'Admin')

  const [customers, setCustomers] = useState<CustomerResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [deleteErrorMessage, setDeleteErrorMessage] = useState<string | null>(null)

  const [formData, setFormData] = useState<CustomerFormState>(
    initialCustomerFormData
  )
  const [isCreating, setIsCreating] = useState(false)
  const [createErrorMessage, setCreateErrorMessage] = useState<string | null>(
    null
  )
  const [createSuccessMessage, setCreateSuccessMessage] = useState<
    string | null
  >(null)

  useEffect(() => {
    let isMounted = true

    getCustomers()
      .then((loadedCustomers) => {
        if (!isMounted) {
          return
        }

        setCustomers(loadedCustomers)
        setErrorMessage(null)
      })
      .catch((error) => {
        if (!isMounted) {
          return
        }

        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Ein unbekannter Fehler ist aufgetreten.'
        )
      })
      .finally(() => {
        if (!isMounted) {
          return
        }

        setIsLoading(false)
      })

    return () => {
      isMounted = false
    }
  }, [])

  async function refreshCustomers() {
    const loadedCustomers = await getCustomers()

    setCustomers(loadedCustomers)
    setErrorMessage(null)
  }

  function handleFormChange(field: keyof CustomerFormState, value: string) {
    setFormData({
      ...formData,
      [field]: value,
    })
  }

  async function handleCreateCustomer(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    setIsCreating(true)
    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)

    try {
      await createCustomer(toCreateCustomerRequest(formData))
      setFormData(initialCustomerFormData)
      setCreateSuccessMessage('Kunde wurde erfolgreich angelegt.')
      await refreshCustomers()
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

  async function handleDeleteCustomer(customerId: string) {
    const confirmed = window.confirm(
      'Möchtest du diesen Kunden wirklich löschen?'
    )

    if (!confirmed) {
      return
    }

    setDeleteErrorMessage(null)

    try {
      await deleteCustomer(customerId)
      await refreshCustomers()
    } catch (error) {
      setDeleteErrorMessage(
        error instanceof Error
          ? error.message
          : 'Ein unbekannter Fehler ist aufgetreten.'
      )
    }
  }

  return (
    <section className="page">
      <PageHeader
        title="Kunden"
        description="Verwalte Kundenstammdaten für Angebote und Aufträge."
      />

      <section className="customer-form-card">
        <h3>Neuen Kunden anlegen</h3>

        <form className="customer-form" onSubmit={handleCreateCustomer}>
          <div className="customer-form-grid">
            <label className="form-field">
              <span>Vorname</span>
              <input
                value={formData.firstName}
                onChange={(event) =>
                  handleFormChange('firstName', event.target.value)
                }
                required
              />
            </label>

            <label className="form-field">
              <span>Nachname</span>
              <input
                value={formData.lastName}
                onChange={(event) =>
                  handleFormChange('lastName', event.target.value)
                }
                required
              />
            </label>

            <label className="form-field">
              <span>Firma</span>
              <input
                value={formData.company}
                onChange={(event) =>
                  handleFormChange('company', event.target.value)
                }
              />
            </label>

            <label className="form-field">
              <span>E-Mail</span>
              <input
                type="email"
                value={formData.email}
                onChange={(event) =>
                  handleFormChange('email', event.target.value)
                }
              />
            </label>

            <label className="form-field">
              <span>Telefon</span>
              <input
                value={formData.phoneNumber}
                onChange={(event) =>
                  handleFormChange('phoneNumber', event.target.value)
                }
              />
            </label>

            <label className="form-field">
              <span>Ort</span>
              <input
                value={formData.city}
                onChange={(event) =>
                  handleFormChange('city', event.target.value)
                }
              />
            </label>

            <label className="form-field">
              <span>Straße</span>
              <input
                value={formData.street}
                onChange={(event) =>
                  handleFormChange('street', event.target.value)
                }
              />
            </label>

            <label className="form-field">
              <span>Hausnummer</span>
              <input
                value={formData.houseNumber}
                onChange={(event) =>
                  handleFormChange('houseNumber', event.target.value)
                }
              />
            </label>

            <label className="form-field">
              <span>Postleitzahl</span>
              <input
                value={formData.postalCode}
                onChange={(event) =>
                  handleFormChange('postalCode', event.target.value)
                }
              />
            </label>

            <label className="form-field customer-form__full">
              <span>Notizen</span>
              <textarea
                value={formData.notes}
                onChange={(event) =>
                  handleFormChange('notes', event.target.value)
                }
              />
            </label>
          </div>

          <button type="submit" className="primary-button" disabled={isCreating}>
            {isCreating ? 'Kunde wird gespeichert...' : 'Kunde anlegen'}
          </button>

          {createErrorMessage && (
            <p className="form-message form-message--error">
              {createErrorMessage}
            </p>
          )}

          {createSuccessMessage && (
            <p className="form-message form-message--success">
              {createSuccessMessage}
            </p>
          )}
        </form>
      </section>

      {deleteErrorMessage && (
        <p className="form-message form-message--error">{deleteErrorMessage}</p>
      )}

      {isLoading && <p className="muted-text">Kunden werden geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && customers.length === 0 && (
        <p className="muted-text">Es sind noch keine Kunden vorhanden.</p>
      )}

      {!isLoading && !errorMessage && customers.length > 0 && (
        <div className="customer-list">
          {customers.map((customer) => (
            <article key={customer.id} className="customer-card">
              <div>
                <h3>
                  {customer.company ||
                    `${customer.firstName} ${customer.lastName}`}
                </h3>

                {customer.company && (
                  <p className="muted-text">
                    {customer.firstName} {customer.lastName}
                  </p>
                )}
              </div>

              <dl className="customer-card__details">
                {customer.email && (
                  <>
                    <dt>E-Mail</dt>
                    <dd>{customer.email}</dd>
                  </>
                )}

                {customer.phoneNumber && (
                  <>
                    <dt>Telefon</dt>
                    <dd>{customer.phoneNumber}</dd>
                  </>
                )}

                {customer.city && (
                  <>
                    <dt>Ort</dt>
                    <dd>{customer.city}</dd>
                  </>
                )}
              </dl>

              {isAdmin && (
                <button
                  type="button"
                  className="secondary-danger-button"
                  onClick={() => handleDeleteCustomer(customer.id)}
                >
                  Löschen
                </button>
              )}
            </article>
          ))}
        </div>
      )}
    </section>
  )
}
