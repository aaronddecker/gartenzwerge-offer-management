import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getCustomers } from '../api/customersApi'
import { getOffers, type OfferResponse } from '../api/offersApi'
import { getOrders, type OrderResponse } from '../api/ordersApi'
import { AnimatedNumber } from '../shared/components/AnimatedNumber'
import { PageHeader } from '../shared/components/PageHeader'
import { QuickActionLink } from '../shared/components/QuickActionLink'
import { StatCard } from '../shared/components/StatCard'
import {
  getRelatedOrder,
  isActiveOrder,
  isOpenOffer,
} from '../shared/businessRules'

const UPCOMING_ORDERS_LIMIT = 5

function formatDate(value: string) {
  return new Intl.DateTimeFormat('de-DE').format(new Date(value))
}

function getOfferByOfferId(offers: OfferResponse[], offerId: string) {
  return offers.find((offer) => offer.id === offerId)
}

function getUpcomingOrders(orders: OrderResponse[]) {
  return orders
    .filter(isActiveOrder)
    .filter((order) => order.plannedDate !== null)
    .sort(
      (a, b) =>
        new Date(a.plannedDate as string).getTime() -
        new Date(b.plannedDate as string).getTime()
    )
    .slice(0, UPCOMING_ORDERS_LIMIT)
}

export function DashboardPage() {
  const [customerCount, setCustomerCount] = useState(0)
  const [offers, setOffers] = useState<OfferResponse[]>([])
  const [orders, setOrders] = useState<OrderResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    Promise.all([getCustomers(), getOffers(), getOrders()])
      .then(([loadedCustomers, loadedOffers, loadedOrders]) => {
        if (!isMounted) {
          return
        }

        setCustomerCount(loadedCustomers.length)
        setOffers(loadedOffers)
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
  }, [])

  const openOfferCount = offers.filter((offer) =>
    isOpenOffer(offer, getRelatedOrder(orders, offer.id))
  ).length

  const activeOrderCount = orders.filter(isActiveOrder).length

  const upcomingOrders = getUpcomingOrders(orders)

  return (
    <section className="page">
      <PageHeader
        title="Dashboard"
        description="Dein Überblick über Kunden, Angebote und anstehende Aufträge."
      />

      {isLoading && <p className="muted-text">Dashboard wird geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && (
        <>
          <div className="stat-grid">
            <StatCard
              title="Kunden"
              value={<AnimatedNumber value={customerCount} />}
              description="Aktuell angelegte Kunden"
            />
            <StatCard
              title="Offene Angebote"
              value={<AnimatedNumber value={openOfferCount} />}
              description="Angebote, die noch nicht angenommen oder abgelehnt wurden"
            />
            <StatCard
              title="Anstehende Aufträge"
              value={<AnimatedNumber value={activeOrderCount} />}
              description="Geplante Aufträge, die noch erledigt werden müssen"
            />
          </div>

          <section className="dashboard-section">
            <h3>Schnellaktion</h3>

            <div className="quick-action-grid">
              <QuickActionLink
                to="/offers"
                title="Angebote öffnen"
                description="Angebote ansehen und später neue Angebote erstellen"
              />
            </div>
          </section>

          <section className="dashboard-section">
            <h3>Anstehende Aufträge</h3>

            {upcomingOrders.length === 0 && (
              <p className="muted-text">
                Aktuell sind keine geplanten Aufträge mit Datum vorhanden.
              </p>
            )}

            {upcomingOrders.length > 0 && (
              <ul className="dashboard-upcoming-list">
                {upcomingOrders.map((order) => {
                  const relatedOffer = getOfferByOfferId(offers, order.offerId)

                  return (
                    <li key={order.id}>
                      <Link
                        to={`/orders/${order.id}`}
                        className="dashboard-upcoming-item"
                      >
                        <div>
                          <span className="dashboard-upcoming-item__customer">
                            {relatedOffer?.customerName ?? 'Auftrag'}
                          </span>
                          <span className="dashboard-upcoming-item__meta">
                            {relatedOffer?.offerNumber ??
                              order.offerId.slice(0, 8)}
                          </span>
                        </div>

                        <span className="dashboard-upcoming-item__date">
                          {formatDate(order.plannedDate as string)}
                        </span>
                      </Link>
                    </li>
                  )
                })}
              </ul>
            )}
          </section>
        </>
      )}
    </section>
  )
}
