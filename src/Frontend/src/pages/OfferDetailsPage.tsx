import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  createOrderFromOffer,
  getOrders,
  type OrderResponse,
} from '../api/ordersApi'
import {
  createOfferItem,
  getOfferItems,
  type OfferItemResponse,
} from '../api/offerItemsApi'
import {
  getOfferById,
  updateOffer,
  type OfferResponse,
  type OfferStatus,
} from '../api/offersApi'
import {
  getOfferedServices,
  type OfferedServiceResponse,
} from '../api/offeredServicesApi'
import { PageHeader } from '../shared/components/PageHeader'

type OfferItemFormState = {
  offeredServiceId: string
  quantity: string
}

const initialOfferItemFormData: OfferItemFormState = {
  offeredServiceId: '',
  quantity: '',
}

function formatCurrency(value: number) {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(value)
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('de-DE').format(new Date(value))
}

function getOfferStatusLabel(status: OfferStatus) {
  switch (status) {
    case 1:
      return 'Entwurf'
    case 2:
      return 'Gesendet'
    case 3:
      return 'Angenommen'
    case 4:
      return 'Abgelehnt'
    default:
      return 'Unbekannt'
  }
}

export function OfferDetailsPage() {
  const navigate = useNavigate()
  const { offerId } = useParams<{ offerId: string }>()

  const [offer, setOffer] = useState<OfferResponse | null>(null)
  const [offerItems, setOfferItems] = useState<OfferItemResponse[]>([])
  const [offeredServices, setOfferedServices] = useState<
    OfferedServiceResponse[]
  >([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const [formData, setFormData] = useState<OfferItemFormState>(
    initialOfferItemFormData
  )
  const [isCreating, setIsCreating] = useState(false)
  const [createErrorMessage, setCreateErrorMessage] = useState<string | null>(
    null
  )
  const [createSuccessMessage, setCreateSuccessMessage] = useState<
    string | null
  >(null)
  const [isConvertingToOrder, setIsConvertingToOrder] = useState(false)
  const [conversionErrorMessage, setConversionErrorMessage] = useState<
    string | null
  >(null)
  const [orders, setOrders] = useState<OrderResponse[]>([])

  useEffect(() => {
    let isMounted = true

    if (!offerId) {
      return
    }

    Promise.all([
      getOfferById(offerId),
      getOfferItems(offerId),
      getOfferedServices(),
      getOrders(),
    ])
      .then(
      ([loadedOffer, loadedOfferItems, loadedOfferedServices, loadedOrders]) => {
        if (!isMounted) {
          return
        }

        setOffer(loadedOffer)
        setOfferItems(loadedOfferItems)
        setOfferedServices(loadedOfferedServices)
        setOrders(loadedOrders)
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
  }, [offerId])

  async function refreshOfferDetails() {
    if (!offerId) {
      return
    }

    const [loadedOffer, loadedOfferItems] = await Promise.all([
      getOfferById(offerId),
      getOfferItems(offerId),
    ])

    setOffer(loadedOffer)
    setOfferItems(loadedOfferItems)
    setErrorMessage(null)
  }

  function handleFormChange(field: keyof OfferItemFormState, value: string) {
    setFormData({
      ...formData,
      [field]: value,
    })
  }

  async function handleCreateOfferItem(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!offerId) {
      return
    }

    setIsCreating(true)
    setCreateErrorMessage(null)
    setCreateSuccessMessage(null)

    try {
      await createOfferItem(offerId, {
        offeredServiceId: formData.offeredServiceId,
        quantity: Number(formData.quantity),
      })

      setFormData(initialOfferItemFormData)
      setCreateSuccessMessage('Position wurde erfolgreich hinzugefügt.')
      await refreshOfferDetails()
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

  async function handleAcceptOfferAndCreateOrder() {
    if (!offerId || !offer) {
      return
    }
    if (existingOrder) {
      setConversionErrorMessage(
        'Für dieses Angebot wurde bereits ein Auftrag erstellt.'
      )
      return
    }

    const confirmed = window.confirm(
      'Möchtest du dieses Angebot annehmen und daraus einen Auftrag erstellen?'
    )

    if (!confirmed) {
      return
    }

    setIsConvertingToOrder(true)
    setConversionErrorMessage(null)

    try {
      if (offer.status !== 3) {
        await updateOffer(offerId, {
          validUntil: new Date(offer.validUntil).toISOString(),
          status: 3,
          notes: offer.notes ?? null,
        })
      }

      await createOrderFromOffer(offerId)
      navigate('/orders')
    } catch (error) {
      setConversionErrorMessage(
        error instanceof Error
          ? error.message
          : 'Ein unbekannter Fehler ist aufgetreten.'
      )
    } finally {
      setIsConvertingToOrder(false)
    }
  }

  const activeOfferedServices = offeredServices.filter(
    (offeredService) => offeredService.isActive
  )

  const existingOrder = offerId
  ? orders.find((order) => order.offerId === offerId)
  : undefined

  const isOfferReadOnly =
    offer?.status === 3 || offer?.status === 4 || existingOrder !== undefined

  if (!offerId) {
    return (
        <section className="page">
        <PageHeader
            title="Angebotsdetails"
            description="Verwalte Positionen und prüfe die Angebotsdaten."
        />

        <p className="form-message form-message--error">
            Angebot konnte nicht gefunden werden.
        </p>
        </section>
    )
    }

  return (
    <section className="page">
      <PageHeader
        title="Angebotsdetails"
        description="Verwalte Positionen und prüfe die Angebotsdaten."
      />

      <div className="page-actions page-actions--left">
        <Link to="/offers" className="secondary-link-button">
          Zurück zu Angeboten
        </Link>
      </div>

      {isLoading && <p className="muted-text">Angebot wird geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && offer && (
        <>
          <section className="offer-summary-card">
            <div>
              <h3>{offer.offerNumber}</h3>
              <p className="muted-text">{offer.customerName}</p>
            </div>

            <dl className="offer-card__details">
              <dt>Status</dt>
              <dd>{getOfferStatusLabel(offer.status)}</dd>

              <dt>Gültig bis</dt>
              <dd>{formatDate(offer.validUntil)}</dd>

              <dt>Gesamt netto</dt>
              <dd>{formatCurrency(offer.totalNet)}</dd>
            </dl>

            {offer.notes && <p className="muted-text">{offer.notes}</p>}
          </section>

          <section className="offer-conversion-card">
            {existingOrder ? (
              <>
                <p className="muted-text">
                  Für dieses Angebot wurde bereits ein Auftrag erstellt. Das Angebot bleibt
                  als Grundlage erhalten und kann nicht mehr verändert werden.
                </p>

                <Link to={`/orders/${existingOrder.id}`} className="secondary-link-button">
                  Auftrag öffnen
                </Link>
              </>
            ) : null}

            {!existingOrder && (offer.status === 1 || offer.status === 2) ? (
              <>
                <p className="muted-text">
                  Wenn der Kunde dieses Angebot angenommen hat, kannst du daraus direkt
                  einen Auftrag erstellen.
                </p>

                <button
                  type="button"
                  className="primary-button"
                  disabled={isConvertingToOrder}
                  onClick={handleAcceptOfferAndCreateOrder}
                >
                  {isConvertingToOrder
                    ? 'Auftrag wird erstellt...'
                    : 'Angebot annehmen & Auftrag erstellen'}
                </button>
              </>
            ) : null}

            {!existingOrder && offer.status === 3 ? (
              <>
                <p className="muted-text">
                  Dieses Angebot ist angenommen. Du kannst daraus einen Auftrag erstellen.
                </p>

                <button
                  type="button"
                  className="primary-button"
                  disabled={isConvertingToOrder}
                  onClick={handleAcceptOfferAndCreateOrder}
                >
                  {isConvertingToOrder
                    ? 'Auftrag wird erstellt...'
                    : 'Auftrag erstellen'}
                </button>
              </>
            ) : null}

            {!existingOrder && offer.status === 4 ? (
              <p className="muted-text">
                Dieses Angebot wurde abgelehnt und kann nicht direkt in einen Auftrag
                umgewandelt werden.
              </p>
            ) : null}

            {conversionErrorMessage && (
              <p className="form-message form-message--error">
                {conversionErrorMessage}
              </p>
            )}
          </section>

          <section className="offer-form-card">
            <h3>Position hinzufügen</h3>

            {isOfferReadOnly ? (
              <p className="muted-text">
                Dieses Angebot ist abgeschlossen und kann nicht mehr verändert werden.
              </p>
            ) : activeOfferedServices.length === 0 ? (
              <p className="muted-text">
                Es sind keine aktiven Leistungen vorhanden.
              </p>
            ) : (
              <form className="offer-form" onSubmit={handleCreateOfferItem}>
                <div className="offer-form-grid">
                  <label className="form-field">
                    <span>Leistung</span>
                    <select
                      value={formData.offeredServiceId}
                      onChange={(event) =>
                        handleFormChange('offeredServiceId', event.target.value)
                      }
                      required
                    >
                      <option value="">Leistung auswählen</option>

                      {activeOfferedServices.map((offeredService) => (
                        <option key={offeredService.id} value={offeredService.id}>
                          {offeredService.name} ·{' '}
                          {formatCurrency(offeredService.basePrice)} /{' '}
                          {offeredService.unit}
                        </option>
                      ))}
                    </select>
                  </label>

                  <label className="form-field">
                    <span>Menge</span>
                    <input
                      type="number"
                      min="0.01"
                      step="0.01"
                      value={formData.quantity}
                      onChange={(event) =>
                        handleFormChange('quantity', event.target.value)
                      }
                      required
                    />
                  </label>
                </div>

                <button
                  type="submit"
                  className="primary-button"
                  disabled={isCreating}
                >
                  {isCreating
                    ? 'Position wird gespeichert...'
                    : 'Position hinzufügen'}
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
            )}
          </section>

          <section className="offer-items-section">
            <h3>Positionen</h3>

            {offerItems.length === 0 && (
              <p className="muted-text">
                Dieses Angebot hat noch keine Positionen.
              </p>
            )}

            {offerItems.length > 0 && (
              <div className="offer-item-list">
                {offerItems.map((offerItem) => (
                  <article key={offerItem.id} className="offer-item-card">
                    <div>
                      <h4>{offerItem.description}</h4>
                      <p className="muted-text">
                        {offerItem.quantity} {offerItem.unit} ×{' '}
                        {formatCurrency(offerItem.unitPrice)}
                      </p>
                    </div>

                    <strong>{formatCurrency(offerItem.totalPrice)}</strong>
                  </article>
                ))}
              </div>
            )}
          </section>
        </>
      )}
    </section>
  )
}