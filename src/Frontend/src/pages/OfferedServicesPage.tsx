import { useEffect, useState, type FormEvent } from 'react'
import {
  createOfferedService,
  getOfferedServices,
  type CreateOfferedServiceRequest,
  type OfferedServiceResponse,
} from '../api/offeredServicesApi'
import { PageHeader } from '../shared/components/PageHeader'
import { PageState } from '../shared/components/PageState'

type OfferedServiceFormState = {
  name: string
  description: string
  unit: string
  basePrice: string
  isActive: boolean
}

const initialOfferedServiceFormData: OfferedServiceFormState = {
  name: '',
  description: '',
  unit: '',
  basePrice: '',
  isActive: true,
}

function emptyToNull(value: string) {
  const trimmedValue = value.trim()

  return trimmedValue.length > 0 ? trimmedValue : null
}

function toCreateOfferedServiceRequest(
  formData: OfferedServiceFormState
): CreateOfferedServiceRequest {
  return {
    name: formData.name.trim(),
    description: emptyToNull(formData.description),
    unit: formData.unit.trim(),
    basePrice: Number(formData.basePrice),
    isActive: formData.isActive,
  }
}

function formatCurrency(value: number) {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(value)
}

export function OfferedServicesPage() {
  const [offeredServices, setOfferedServices] = useState<
    OfferedServiceResponse[]
  >([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const [formData, setFormData] = useState<OfferedServiceFormState>(
    initialOfferedServiceFormData
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

    getOfferedServices()
      .then((loadedOfferedServices) => {
        if (!isMounted) {
          return
        }

        setOfferedServices(loadedOfferedServices)
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

  async function refreshOfferedServices() {
    const loadedOfferedServices = await getOfferedServices()

    setOfferedServices(loadedOfferedServices)
    setErrorMessage(null)
  }

  function handleFormChange(
    field: keyof OfferedServiceFormState,
    value: string | boolean
  ) {
    setFormData({
      ...formData,
      [field]: value,
    })
  }

  async function handleCreateOfferedService(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault()

    setIsCreating(true)
    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)

    try {
      await createOfferedService(toCreateOfferedServiceRequest(formData))
      setFormData(initialOfferedServiceFormData)
      setCreateSuccessMessage('Leistung wurde erfolgreich angelegt.')
      await refreshOfferedServices()
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
        title="Leistungen"
        description="Verwalte angebotene Leistungen mit Einheit und Basispreis."
      />

      <section className="service-form-card">
        <h3>Neue Leistung anlegen</h3>

        <form className="service-form" onSubmit={handleCreateOfferedService}>
          <div className="service-form-grid">
            <label className="form-field">
              <span>Name</span>
              <input
                value={formData.name}
                onChange={(event) =>
                  handleFormChange('name', event.target.value)
                }
                required
              />
            </label>

            <label className="form-field">
              <span>Einheit</span>
              <input
                value={formData.unit}
                onChange={(event) =>
                  handleFormChange('unit', event.target.value)
                }
                placeholder="z. B. m², m, Stück"
                required
              />
            </label>

            <label className="form-field">
              <span>Basispreis</span>
              <input
                type="number"
                min="0"
                step="0.01"
                value={formData.basePrice}
                onChange={(event) =>
                  handleFormChange('basePrice', event.target.value)
                }
                required
              />
            </label>

            <label className="checkbox-field">
              <input
                type="checkbox"
                checked={formData.isActive}
                onChange={(event) =>
                  handleFormChange('isActive', event.target.checked)
                }
              />
              <span>Aktiv</span>
            </label>

            <label className="form-field service-form__full">
              <span>Beschreibung</span>
              <textarea
                value={formData.description}
                onChange={(event) =>
                  handleFormChange('description', event.target.value)
                }
              />
            </label>
          </div>

          <button type="submit" className="primary-button" disabled={isCreating}>
            {isCreating ? 'Leistung wird gespeichert...' : 'Leistung anlegen'}
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

      <PageState
        isLoading={isLoading}
        loadingText="Leistungen werden geladen..."
        error={errorMessage}
      >
      {offeredServices.length === 0 && (
        <section className="empty-state-card">
          <h3>Noch keine Leistungen vorhanden</h3>
          <p>Angelegte Leistungen erscheinen hier.</p>
        </section>
      )}

      {offeredServices.length > 0 && (
        <div className="service-list">
          {offeredServices.map((offeredService) => (
            <article key={offeredService.id} className="service-card">
              <div>
                <h3>{offeredService.name}</h3>

                {offeredService.description && (
                  <p className="muted-text">{offeredService.description}</p>
                )}
              </div>

              <dl className="service-card__details">
                <dt>Einheit</dt>
                <dd>{offeredService.unit}</dd>

                <dt>Basispreis</dt>
                <dd>{formatCurrency(offeredService.basePrice)}</dd>

                <dt>Status</dt>
                <dd>{offeredService.isActive ? 'Aktiv' : 'Inaktiv'}</dd>
              </dl>
            </article>
          ))}
        </div>
      )}
      </PageState>
    </section>
  )
}
