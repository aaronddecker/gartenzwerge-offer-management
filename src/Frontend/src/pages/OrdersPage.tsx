import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getOffers, type OfferResponse } from '../api/offersApi'
import {
  getOrders,
  type OrderResponse,
  type OrderStatus,
} from '../api/ordersApi'
import { PageHeader } from '../shared/components/PageHeader'
import { PageState } from '../shared/components/PageState'
import { isActiveOrder, isDoneOrder } from '../shared/businessRules'

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

type OrderFilter = 'active' | 'done' | 'all'

export function OrdersPage() {
  const [orders, setOrders] = useState<OrderResponse[]>([])
  const [offers, setOffers] = useState<OfferResponse[]>([])
  const [activeFilter, setActiveFilter] = useState<OrderFilter>('active')
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

  const activeOrders = orders.filter(isActiveOrder)
  const doneOrders = orders.filter(isDoneOrder)

  const filteredOrders = orders.filter((order) => {
    if (activeFilter === 'active') {
      return isActiveOrder(order)
    }

    if (activeFilter === 'done') {
      return isDoneOrder(order)
    }

    return true
  })

  return (
    <section className="page">
      <PageHeader
        title="Aufträge"
        description="Verwalte angenommene Angebote, die als Aufträge weiterbearbeitet werden."
      />

      <div className="order-filter-tabs" aria-label="Auftragsfilter">
        <button
          type="button"
          className={
            activeFilter === 'active'
              ? 'order-filter-tab order-filter-tab--active'
              : 'order-filter-tab'
          }
          onClick={() => setActiveFilter('active')}
        >
          Aktiv
          <span>{activeOrders.length}</span>
        </button>

        <button
          type="button"
          className={
            activeFilter === 'done'
              ? 'order-filter-tab order-filter-tab--active'
              : 'order-filter-tab'
          }
          onClick={() => setActiveFilter('done')}
        >
          Abgeschlossen
          <span>{doneOrders.length}</span>
        </button>

        <button
          type="button"
          className={
            activeFilter === 'all'
              ? 'order-filter-tab order-filter-tab--active'
              : 'order-filter-tab'
          }
          onClick={() => setActiveFilter('all')}
        >
          Alle
          <span>{orders.length}</span>
        </button>
      </div>

      <PageState
        isLoading={isLoading}
        loadingText="Aufträge werden geladen..."
        error={errorMessage}
      >
      {orders.length === 0 && (
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

      {orders.length > 0 && filteredOrders.length === 0 && (
        <section className="empty-state-card">
          {activeFilter === 'active' ? (
            <>
              <h3>Keine aktiven Aufträge</h3>
              <p>Geplante oder in Bearbeitung befindliche Aufträge erscheinen hier.</p>
            </>
          ) : (
            <>
              <h3>Keine abgeschlossenen Aufträge</h3>
              <p>Abgeschlossene oder stornierte Aufträge erscheinen hier.</p>
            </>
          )}
        </section>
      )}

      {filteredOrders.length > 0 && (
        <div className="order-list">
          {filteredOrders.map((order) => {
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

                  <span className={`order-status-badge order-status-badge--${order.status}`}>
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
      </PageState>
    </section>
  )
}