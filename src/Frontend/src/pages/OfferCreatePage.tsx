import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import {
  createCustomer,
  getCustomers,
  type CreateCustomerRequest,
  type CustomerResponse,
} from '../api/customersApi'
import { createOffer, type CreateOfferRequest } from '../api/offersApi'
import { PageHeader } from '../shared/components/PageHeader'

type OfferCreateFormState = {
  customerSearchTerm: string
  selectedCustomerId: string
  firstName: string
  lastName: string
  company: string
  phoneNumber: string
  email: string
  street: string
  houseNumber: string
  postalCode: string
  city: string
  customerNotes: string
  validUntil: string
  offerNotes: string
}

type OfferCreateTextField = keyof OfferCreateFormState

const initialOfferCreateFormData: OfferCreateFormState = {
  customerSearchTerm: '',
  selectedCustomerId: '',
  firstName: '',
  lastName: '',
  company: '',
  phoneNumber: '',
  email: '',
  street: '',
  houseNumber: '',
  postalCode: '',
  city: '',
  customerNotes: '',
  validUntil: '',
  offerNotes: '',
}

function emptyToNull(value: string) {
  const trimmedValue = value.trim()

  return trimmedValue.length > 0 ? trimmedValue : null
}

function toCreateCustomerRequest(
  formData: OfferCreateFormState
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
    notes: emptyToNull(formData.customerNotes),
  }
}

function toCreateOfferRequest(
  customerId: string,
  formData: OfferCreateFormState
): CreateOfferRequest {
  return {
    customerId,
    validUntil: new Date(`${formData.validUntil}T00:00:00.000Z`).toISOString(),
    notes: emptyToNull(formData.offerNotes),
  }
}

function getCustomerDisplayName(customer: CustomerResponse) {
  return customer.company || `${customer.firstName} ${customer.lastName}`
}

function getCustomerSearchText(customer: CustomerResponse) {
  return [
    customer.firstName,
    customer.lastName,
    customer.company,
    customer.email,
    customer.phoneNumber,
    customer.city,
  ]
    .filter(Boolean)
    .join(' ')
    .toLowerCase()
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

  const trimmedSearchTerm = formData.customerSearchTerm.trim().toLowerCase()

  const selectedCustomer = customers.find(
    (customer) => customer.id === formData.selectedCustomerId
  )

  const matchingCustomers =
    trimmedSearchTerm.length >= 2 && !selectedCustomer
      ? customers
          .filter((customer) =>
            getCustomerSearchText(customer).includes(trimmedSearchTerm)
          )
          .slice(0, 5)
      : []

  const shouldShowNewCustomerFields =
    trimmedSearchTerm.length >= 3 &&
    !selectedCustomer &&
    matchingCustomers.length === 0

  function handleFormChange(field: OfferCreateTextField, value: string) {
    setFormData((currentFormData) => ({
      ...currentFormData,
      [field]: value,
    }))
  }

  function handleCustomerSearchChange(value: string) {
    setFormData((currentFormData) => ({
      ...currentFormData,
      customerSearchTerm: value,
      selectedCustomerId: '',
    }))

    setCreateErrorMessage(null)
  }

  function handleSelectCustomer(customer: CustomerResponse) {
    setFormData((currentFormData) => ({
      ...currentFormData,
      customerSearchTerm: getCustomerDisplayName(customer),
      selectedCustomerId: customer.id,
      firstName: '',
      lastName: '',
      company: '',
      phoneNumber: '',
      email: '',
      street: '',
      houseNumber: '',
      postalCode: '',
      city: '',
      customerNotes: '',
    }))

    setCreateErrorMessage(null)
  }

  async function handleCreateOffer(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    setIsCreating(true)
    setCreateErrorMessage(null)

    try {
      let customerId = formData.selectedCustomerId

      if (!customerId && !shouldShowNewCustomerFields) {
        setCreateErrorMessage(
          'Bitte wähle einen bestehenden Kunden aus oder gib einen neuen Kunden ein.'
        )
        return
      }

      if (!customerId && shouldShowNewCustomerFields) {
        const createdCustomer = await createCustomer(
          toCreateCustomerRequest(formData)
        )

        customerId = createdCustomer.id
      }

      await createOffer(toCreateOfferRequest(customerId, formData))
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
        description="Suche einen bestehenden Kunden oder lege ihn direkt im Angebotsprozess neu an."
      />

      <div className="page-actions page-actions--left">
        <Link to="/offers" className="secondary-link-button">
          Zurück zu Angeboten
        </Link>
      </div>

      <section className="offer-form-card">
        <form className="offer-form" onSubmit={handleCreateOffer}>
          <section className="offer-form-section">
            <h3>Kunde</h3>

            {isLoadingCustomers && (
              <p className="muted-text">Kunden werden geladen...</p>
            )}

            {!isLoadingCustomers && customerErrorMessage && (
              <p className="form-message form-message--error">
                {customerErrorMessage}
              </p>
            )}

            {!isLoadingCustomers && !customerErrorMessage && (
              <>
                <label className="form-field">
                  <span>Kunde suchen oder neu anlegen</span>
                  <input
                    value={formData.customerSearchTerm}
                    onChange={(event) =>
                      handleCustomerSearchChange(event.target.value)
                    }
                    placeholder="Name, Firma, E-Mail, Telefon oder Ort eingeben"
                    required
                  />
                </label>

                {!selectedCustomer &&
                  trimmedSearchTerm.length > 0 &&
                  trimmedSearchTerm.length < 3 && (
                    <p className="muted-text">
                      Gib mindestens 3 Zeichen ein, um nach Kunden zu suchen.
                    </p>
                  )}

                {matchingCustomers.length > 0 && (
                  <div className="customer-suggestion-list">
                    {matchingCustomers.map((customer) => (
                      <button
                        key={customer.id}
                        type="button"
                        className="customer-suggestion-button"
                        onClick={() => handleSelectCustomer(customer)}
                      >
                        <strong>{getCustomerDisplayName(customer)}</strong>

                        <span>
                          {[customer.email, customer.phoneNumber, customer.city]
                            .filter(Boolean)
                            .join(' · ')}
                        </span>
                      </button>
                    ))}
                  </div>
                )}

                {selectedCustomer && (
                  <article className="selected-customer-card">
                    <h4>{getCustomerDisplayName(selectedCustomer)}</h4>

                    <dl className="customer-card__details">
                      {selectedCustomer.email && (
                        <>
                          <dt>E-Mail</dt>
                          <dd>{selectedCustomer.email}</dd>
                        </>
                      )}

                      {selectedCustomer.phoneNumber && (
                        <>
                          <dt>Telefon</dt>
                          <dd>{selectedCustomer.phoneNumber}</dd>
                        </>
                      )}

                      {selectedCustomer.city && (
                        <>
                          <dt>Ort</dt>
                          <dd>{selectedCustomer.city}</dd>
                        </>
                      )}
                    </dl>
                  </article>
                )}

                {shouldShowNewCustomerFields && (
                  <>
                    <p className="muted-text">
                      Kein passender Kunde gefunden. Lege den Kunden direkt für
                      dieses Angebot neu an.
                    </p>

                    <div className="offer-form-grid">
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

                      <label className="form-field offer-form__full">
                        <span>Kundennotizen</span>
                        <textarea
                          value={formData.customerNotes}
                          onChange={(event) =>
                            handleFormChange(
                              'customerNotes',
                              event.target.value
                            )
                          }
                        />
                      </label>
                    </div>
                  </>
                )}
              </>
            )}
          </section>

          <section className="offer-form-section">
            <h3>Angebotsdaten</h3>

            <div className="offer-form-grid">
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
                <span>Angebotsnotizen</span>
                <textarea
                  value={formData.offerNotes}
                  onChange={(event) =>
                    handleFormChange('offerNotes', event.target.value)
                  }
                />
              </label>
            </div>
          </section>

          <button type="submit" className="primary-button" disabled={isCreating}>
            {isCreating ? 'Angebot wird gespeichert...' : 'Angebot anlegen'}
          </button>

          {createErrorMessage && (
            <p className="form-message form-message--error">
              {createErrorMessage}
            </p>
          )}
        </form>
      </section>
    </section>
  )
}
