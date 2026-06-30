import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getOfferItems, type OfferItemResponse } from '../api/offerItemsApi'
import { getOfferById, type OfferResponse } from '../api/offersApi'
import {
  getOrderById,
  updateOrder,
  type OrderResponse,
  type OrderStatus,
} from '../api/ordersApi'
import { PageHeader } from '../shared/components/PageHeader'

function formatCurrency(value: number) {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(value)
}

function formatDate(value: string | null) {
  if (!value) {
    return '–'
  }

  return new Intl.DateTimeFormat('de-DE').format(new Date(value))
}

function getOrderStatusLabel(status: OrderStatus) {
  switch (status) {
    case 1:
      return 'Geplant'
    case 2:
      return 'In Bearbeitung'
    case 3:
      return 'Abgeschlossen'
    case 4:
      return 'Storniert'
    default:
      return 'Unbekannt'
  }
}

function toDateInputValue(value: string | null): string {
  if (!value) return ''
  return value.slice(0, 10)
}

const ORDER_STATUS_OPTIONS: { value: OrderStatus; label: string }[] = [
  { value: 1, label: 'Geplant' },
  { value: 2, label: 'In Bearbeitung' },
  { value: 3, label: 'Abgeschlossen' },
  { value: 4, label: 'Storniert' },
]

export function OrderDetailsPage() {
  const { orderId } = useParams<{ orderId: string }>()

  const [order, setOrder] = useState<OrderResponse | null>(null)
  const [offer, setOffer] = useState<OfferResponse | null>(null)
  const [offerItems, setOfferItems] = useState<OfferItemResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const [formStatus, setFormStatus] = useState<OrderStatus>(1)
  const [formPlannedDate, setFormPlannedDate] = useState('')
  const [formNotes, setFormNotes] = useState('')
  const [isSaving, setIsSaving] = useState(false)
  const [saveError, setSaveError] = useState<string | null>(null)
  const [saveSuccess, setSaveSuccess] = useState(false)

  useEffect(() => {
    let isMounted = true

    if (!orderId) {
      return
    }

    const currentOrderId = orderId

    async function loadOrderDetails() {
      try {
        const loadedOrder = await getOrderById(currentOrderId)

        const [loadedOffer, loadedOfferItems] = await Promise.all([
          getOfferById(loadedOrder.offerId),
          getOfferItems(loadedOrder.offerId),
        ])

        if (!isMounted) {
          return
        }

        setOrder(loadedOrder)
        setOffer(loadedOffer)
        setOfferItems(loadedOfferItems)
        setFormStatus(loadedOrder.status)
        setFormPlannedDate(toDateInputValue(loadedOrder.plannedDate))
        setFormNotes(loadedOrder.notes ?? '')
        setErrorMessage(null)
      } catch (error) {
        if (!isMounted) {
          return
        }

        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Ein unbekannter Fehler ist aufgetreten.'
        )
      } finally {
        if (isMounted) {
          setIsLoading(false)
        }
      }
    }

    loadOrderDetails()

    return () => {
      isMounted = false
    }
  }, [orderId])

  async function handleSave() {
    if (!orderId) return

    setIsSaving(true)
    setSaveError(null)
    setSaveSuccess(false)

    try {
      const updatedOrder = await updateOrder(orderId, {
        status: formStatus,
        plannedDate: formPlannedDate ? `${formPlannedDate}T00:00:00.000Z` : null,
        notes: formNotes.trim() || null,
      })

      setOrder(updatedOrder)
      setFormStatus(updatedOrder.status)
      setFormPlannedDate(toDateInputValue(updatedOrder.plannedDate))
      setFormNotes(updatedOrder.notes ?? '')
      setSaveSuccess(true)
    } catch (error) {
      setSaveError(
        error instanceof Error
          ? error.message
          : 'Auftrag konnte nicht gespeichert werden.'
      )
    } finally {
      setIsSaving(false)
    }
  }

  if (!orderId) {
    return (
      <section className="page">
        <PageHeader
          title="Auftragsdetails"
          description="Prüfe die Auftragsdaten und bearbeite den Planungsstand."
        />

        <p className="form-message form-message--error">
          Auftrag konnte nicht gefunden werden.
        </p>
      </section>
    )
  }

  return (
    <section className="page">
      <PageHeader
        title="Auftragsdetails"
        description="Prüfe die Auftragsdaten und bearbeite den Planungsstand."
      />

      <div className="page-actions page-actions--left">
        <Link to="/orders" className="secondary-link-button">
          Zurück zu Aufträgen
        </Link>
      </div>

      {isLoading && <p className="muted-text">Auftrag wird geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && order && offer && (
        <>
          <section className="order-detail-card">
            <div className="order-card__header">
              <div>
                <h3>{offer.offerNumber}</h3>
                <p className="muted-text">{offer.customerName}</p>
              </div>

              <span className={`order-status-badge order-status-badge--${order.status}`}>
                {getOrderStatusLabel(order.status)}
              </span>
            </div>

            <div className="form-field">
              <label htmlFor="order-status">Status</label>
              <select
                id="order-status"
                value={formStatus}
                onChange={(e) => {
                  setFormStatus(Number(e.target.value) as OrderStatus)
                  setSaveSuccess(false)
                }}
              >
                {ORDER_STATUS_OPTIONS.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-field">
              <label htmlFor="order-planned-date">Geplant für</label>
              <input
                id="order-planned-date"
                type="date"
                value={formPlannedDate}
                onChange={(e) => {
                  setFormPlannedDate(e.target.value)
                  setSaveSuccess(false)
                }}
              />
            </div>

            {order.completedAt && (
              <dl className="order-card__details">
                <dt>Abgeschlossen am</dt>
                <dd>{formatDate(order.completedAt)}</dd>
              </dl>
            )}

            <div className="form-field">
              <label htmlFor="order-notes">Notizen</label>
              <textarea
                id="order-notes"
                value={formNotes}
                rows={4}
                maxLength={1000}
                placeholder="Interne Notizen zum Auftrag..."
                onChange={(e) => {
                  setFormNotes(e.target.value)
                  setSaveSuccess(false)
                }}
              />
            </div>

            {saveError && (
              <p className="form-message form-message--error">{saveError}</p>
            )}

            {saveSuccess && (
              <p className="form-message form-message--success">
                Auftrag wurde gespeichert.
              </p>
            )}

            <div className="page-actions page-actions--left">
              <button
                type="button"
                className="primary-button"
                disabled={isSaving}
                onClick={handleSave}
              >
                {isSaving ? 'Wird gespeichert...' : 'Speichern'}
              </button>
            </div>
          </section>

          <section className="order-detail-card">
            <h3>Angebotsgrundlage</h3>

            <dl className="order-card__details">
              <dt>Angebotsnummer</dt>
              <dd>{offer.offerNumber}</dd>

              <dt>Kunde</dt>
              <dd>{offer.customerName}</dd>

              <dt>Gültig bis</dt>
              <dd>{formatDate(offer.validUntil)}</dd>

              <dt>Gesamt netto</dt>
              <dd>{formatCurrency(offer.totalNet)}</dd>
            </dl>

            <div className="page-actions page-actions--left">
              <Link
                to={`/offers/${offer.id}`}
                className="secondary-link-button"
              >
                Zugehöriges Angebot öffnen
              </Link>
            </div>
          </section>

          <section className="offer-items-section">
            <h3>Positionen</h3>

            {offerItems.length === 0 && (
              <p className="muted-text">
                Für die Angebotsgrundlage sind keine Positionen vorhanden.
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
