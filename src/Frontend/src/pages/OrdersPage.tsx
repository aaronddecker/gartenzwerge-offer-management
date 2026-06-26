import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getOffers, type OfferResponse } from '../api/offersApi'
import {
  getOrders,
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

function getOfferById(offers: OfferResponse[], offerId: string) {
  return offers.find((offer) => offer.id === offerId)
}

export function OrdersPage() {
  const [orders, setOrders] = useState<OrderResponse[]>([])
  const [offers, setOffers] = useState<OfferResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    Promise.all([getOrders(), getOffers()])
      .then(([loadedOrders, loadedOffers]) => {
        if (!isMounted) {
          return
        }

        setOrders(loadedOrders)
        setOffers(loadedOffers)
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

  return (
    <section className="page">
      <PageHeader
        title="Aufträge"
        description="Sieh angenommene Angebote, die als Aufträge weiterbearbeitet werden."
      />

      {isLoading && <p className="muted-text">Aufträge werden geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && orders.length === 0 && (
        <section className="empty-state-card">
          <h3>Noch keine Aufträge vorhanden</h3>
          <p>
            Sobald ein angenommenes Angebot in einen Auftrag umgewandelt wurde,
            erscheint es hier.
          </p>

          <Link to="/offers" className="primary-button">
            Zu den Angeboten
          </Link>
        </section>
      )}

      {!isLoading && !errorMessage && orders.length > 0 && (
        <div className="order-list">
          {orders.map((order) => {
            const relatedOffer = getOfferById(offers, order.offerId)

            return (
              <article key={order.id} className="order-card">
                <div className="order-card__header">
                  <div>
                    <h3>
                      {relatedOffer?.offerNumber ??
                        `Auftrag ${order.id.slice(0, 8)}`}
                    </h3>

                    <p className="muted-text">
                      {relatedOffer?.customerName ??
                        `Kunde ${order.customerId.slice(0, 8)}`}
                    </p>
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
                  <dd>
                    {relatedOffer
                      ? formatCurrency(relatedOffer.totalNet)
                      : 'Nicht verfügbar'}
                  </dd>
                </dl>

                {order.notes && <p className="muted-text">{order.notes}</p>}

                <div className="page-actions page-actions--left">
                  <Link
                    to={`/offers/${order.offerId}`}
                    className="secondary-link-button"
                  >
                    Zugehöriges Angebot öffnen
                  </Link>
                </div>
              </article>
            )
          })}
        </div>
      )}
    </section>
  )
}