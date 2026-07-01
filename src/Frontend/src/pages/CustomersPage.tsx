import { Fragment, useEffect, useState, type FormEvent } from 'react'
import { useOutletContext } from 'react-router-dom'
import type { CurrentUserResponse } from '../api/authApi'
import {
  createCustomer,
  deleteCustomer,
  getCustomers,
  updateCustomer,
  type CreateCustomerRequest,
  type CustomerResponse,
} from '../api/customersApi'
import { PageHeader } from '../shared/components/PageHeader'
import { PageState } from '../shared/components/PageState'
import { CustomerForm, type CustomerFormState } from './CustomerForm'

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
  const [deleteErrorMessage, setDeleteErrorMessage] = useState<string | null>(
    null
  )

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

  const [editingCustomerId, setEditingCustomerId] = useState<string | null>(null)
  const isEditing = editingCustomerId !== null

  const [isFormOpen, setIsFormOpen] = useState(false)
  const showForm = isFormOpen || isEditing
  const isCreateFormOpen = isFormOpen && !isEditing

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

  function handleOpenCreateForm() {
    setEditingCustomerId(null)
    setFormData(initialCustomerFormData)
    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)
    setIsFormOpen(true)
  }

  function handleStartEditCustomer(customer: CustomerResponse) {
    setEditingCustomerId(customer.id)
    setIsFormOpen(true)

    setFormData({
      firstName: customer.firstName ?? '',
      lastName: customer.lastName ?? '',
      company: customer.company ?? '',
      phoneNumber: customer.phoneNumber ?? '',
      email: customer.email ?? '',
      street: customer.street ?? '',
      houseNumber: customer.houseNumber ?? '',
      postalCode: customer.postalCode ?? '',
      city: customer.city ?? '',
      notes: customer.notes ?? '',
    })

    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)
  }

  function handleCancelEdit() {
    setEditingCustomerId(null)
    setFormData(initialCustomerFormData)
    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)
    setIsFormOpen(false)
  }

  async function handleSaveCustomer(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    setIsCreating(true)
    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)

    try {
      if (editingCustomerId) {
        await updateCustomer(
          editingCustomerId,
          toCreateCustomerRequest(formData)
        )

        setCreateSuccessMessage('Kunde wurde erfolgreich aktualisiert.')
        setEditingCustomerId(null)
      } else {
        await createCustomer(toCreateCustomerRequest(formData))
        setCreateSuccessMessage('Kunde wurde erfolgreich angelegt.')
      }

      setFormData(initialCustomerFormData)
      await refreshCustomers()
      setIsFormOpen(false)
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

      {!showForm && (
        <div className="page-actions page-actions--left">
          <button
            type="button"
            className="primary-button"
            onClick={handleOpenCreateForm}
          >
            Neuen Kunden anlegen
          </button>
        </div>
      )}

      {isCreateFormOpen && (
        <CustomerForm
          formData={formData}
          isEditing={isEditing}
          isSaving={isCreating}
          errorMessage={createErrorMessage}
          successMessage={createSuccessMessage}
          onFieldChange={handleFormChange}
          onSubmit={handleSaveCustomer}
          onCancel={handleCancelEdit}
        />
      )}

      {deleteErrorMessage && (
        <p className="form-message form-message--error">{deleteErrorMessage}</p>
      )}

      <PageState
        isLoading={isLoading}
        loadingText="Kunden werden geladen..."
        error={errorMessage}
      >
      {customers.length === 0 && (
        <section className="empty-state-card">
          <h3>Noch keine Kunden vorhanden</h3>
          <p>Angelegte Kunden erscheinen hier.</p>
        </section>
      )}

      {customers.length > 0 && (
        <div className="customer-list">
          {customers.map((customer) => (
            <Fragment key={customer.id}>
            <article className="customer-card">
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

              <div className="customer-card__actions">
                <button
                  type="button"
                  className="secondary-button"
                  onClick={() => handleStartEditCustomer(customer)}
                >
                  Bearbeiten
                </button>

                {isAdmin && (
                  <button
                    type="button"
                    className="secondary-danger-button"
                    onClick={() => handleDeleteCustomer(customer.id)}
                  >
                    Löschen
                  </button>
                )}
              </div>
            </article>

            {editingCustomerId === customer.id && (
              <CustomerForm
                formData={formData}
                isEditing={isEditing}
                isSaving={isCreating}
                isInline
                errorMessage={createErrorMessage}
                successMessage={createSuccessMessage}
                onFieldChange={handleFormChange}
                onSubmit={handleSaveCustomer}
                onCancel={handleCancelEdit}
              />
            )}
            </Fragment>
          ))}
        </div>
      )}
      </PageState>
    </section>
  )
}
