import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getOfferItems, type OfferItemResponse } from '../api/offerItemsApi'
import { getOfferById, type OfferResponse } from '../api/offersApi'
import {
  getOrderById,
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
    return 'Noch nicht geplant'
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

export function OrderDetailsPage() {
  const { orderId } = useParams<{ orderId: string }>()

  const [order, setOrder] = useState<OrderResponse | null>(null)
  const [offer, setOffer] = useState<OfferResponse | null>(null)
  const [offerItems, setOfferItems] = useState<OfferItemResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

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

  if (!orderId) {
    return (
      <section className="page">
        <PageHeader
          title="Auftragsdetails"
          description="Prüfe die Auftragsdaten und die zugehörige Angebotsgrundlage."
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
        description="Prüfe die Auftragsdaten und die zugehörige Angebotsgrundlage."
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

              <span className="order-status-badge">
                {getOrderStatusLabel(order.status)}
              </span>
            </div>

            <dl className="order-card__details">
              <dt>Geplant für</dt>
              <dd>{formatDate(order.plannedDate)}</dd>

              <dt>Abgeschlossen am</dt>
              <dd>{formatDate(order.completedAt)}</dd>

              <dt>Gesamt netto</dt>
              <dd>{formatCurrency(offer.totalNet)}</dd>
            </dl>

            {order.notes && <p className="muted-text">{order.notes}</p>}
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