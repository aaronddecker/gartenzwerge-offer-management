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

function formatPlannedDate(value: string | null) {
  if (!value) {
    return 'Noch nicht geplant'
  }

  return new Intl.DateTimeFormat('de-DE').format(new Date(value))
}

function formatCompletedDate(value: string | null) {
  if (!value) {
    return 'Noch nicht abgeschlossen'
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

function getShortId(id: string) {
  return id.slice(0, 8).toUpperCase()
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
        description="Verwalte angenommene Angebote, die als Aufträge weiterbearbeitet werden."
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
                <div className="order-card__top">
                  <div>
                    <span className="order-card__eyebrow">Auftrag</span>
                    <h3>#{getShortId(order.id)}</h3>

                    <p className="order-card__customer">
                      {relatedOffer?.customerName ??
                        `Kunde ${order.customerId.slice(0, 8)}`}
                    </p>

                    <p className="muted-text">
                      Aus Angebot{' '}
                      {relatedOffer?.offerNumber ?? order.offerId.slice(0, 8)}
                    </p>
                  </div>

                  <span className="order-status-badge">
                    {getOrderStatusLabel(order.status)}
                  </span>
                </div>

                <div className="order-card__metrics">
                  <div>
                    <span>Gesamt netto</span>
                    <strong>
                      {relatedOffer
                        ? formatCurrency(relatedOffer.totalNet)
                        : 'Nicht verfügbar'}
                    </strong>
                  </div>

                  <div>
                    <span>Geplant für</span>
                    <strong>{formatPlannedDate(order.plannedDate)}</strong>
                  </div>

                  <div>
                    <span>Abgeschlossen</span>
                    <strong>{formatCompletedDate(order.completedAt)}</strong>
                  </div>
                </div>

                {order.notes && <p className="muted-text">{order.notes}</p>}

                <div className="order-card__actions">
                  <Link to={`/orders/${order.id}`} className="primary-button order-card__action-button">
                    Öffnen
                  </Link>

                  <Link
                    to={`/offers/${order.offerId}`}
                    className="secondary-link-button order-card__action-button"
                  >
                    Zum Angebot
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