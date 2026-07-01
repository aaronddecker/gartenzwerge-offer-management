import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getOffers,
  type OfferResponse,
  type OfferStatus,
} from '../api/offersApi'
import { getOrders, type OrderResponse } from '../api/ordersApi'
import { PageHeader } from '../shared/components/PageHeader'
import {
  getRelatedOrder,
  isArchivedOffer,
  isOpenOffer,
} from '../shared/businessRules'

type OfferFilter = 'open' | 'archive' | 'all'

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

export function OffersPage() {
  const [offers, setOffers] = useState<OfferResponse[]>([])
  const [orders, setOrders] = useState<OrderResponse[]>([])
  const [activeFilter, setActiveFilter] = useState<OfferFilter>('open')
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    Promise.all([getOffers(), getOrders()])
      .then(([loadedOffers, loadedOrders]) => {
        if (!isMounted) {
          return
        }

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

  const openOffers = offers.filter((offer) =>
    isOpenOffer(offer, getRelatedOrder(orders, offer.id))
  )

  const archivedOffers = offers.filter((offer) =>
    isArchivedOffer(offer, getRelatedOrder(orders, offer.id))
  )

  const filteredOffers = offers.filter((offer) => {
    const relatedOrder = getRelatedOrder(orders, offer.id)

    if (activeFilter === 'open') {
      return isOpenOffer(offer, relatedOrder)
    }

    if (activeFilter === 'archive') {
      return isArchivedOffer(offer, relatedOrder)
    }

    return true
  })

  return (
    <section className="page">
      <PageHeader
        title="Angebote"
        description="Verwalte offene Angebote und greife über das Archiv auf abgeschlossene Angebotshistorie zu."
      />

      <div className="page-actions page-actions--left">
        <Link to="/offers/new" className="primary-button">
          Neues Angebot
        </Link>
      </div>

      <div className="offer-filter-tabs" aria-label="Angebotsfilter">
        <button
          type="button"
          className={
            activeFilter === 'open'
              ? 'offer-filter-tab offer-filter-tab--active'
              : 'offer-filter-tab'
          }
          onClick={() => setActiveFilter('open')}
        >
          Offen
          <span>{openOffers.length}</span>
        </button>

        <button
          type="button"
          className={
            activeFilter === 'archive'
              ? 'offer-filter-tab offer-filter-tab--active'
              : 'offer-filter-tab'
          }
          onClick={() => setActiveFilter('archive')}
        >
          Archiv
          <span>{archivedOffers.length}</span>
        </button>

        <button
          type="button"
          className={
            activeFilter === 'all'
              ? 'offer-filter-tab offer-filter-tab--active'
              : 'offer-filter-tab'
          }
          onClick={() => setActiveFilter('all')}
        >
          Alle
          <span>{offers.length}</span>
        </button>
      </div>

      {isLoading && <p className="muted-text">Angebote werden geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && filteredOffers.length === 0 && (
        <section className="empty-state-card">
          {activeFilter === 'open' ? (
            <>
              <h3>Keine offenen Angebote vorhanden</h3>
              <p>
                Neue oder noch nicht umgewandelte Angebote erscheinen hier.
              </p>
            </>
          ) : activeFilter === 'archive' ? (
            <>
              <h3>Das Archiv ist leer</h3>
              <p>
                Abgelehnte oder bereits in Aufträge umgewandelte Angebote
                erscheinen hier.
              </p>
            </>
          ) : (
            <>
              <h3>Noch keine Angebote vorhanden</h3>
              <p>Erstelle dein erstes Angebot für einen Kunden.</p>
            </>
          )}

          {activeFilter !== 'archive' && (
            <Link to="/offers/new" className="primary-button">
              Neues Angebot
            </Link>
          )}
        </section>
      )}

      {!isLoading && !errorMessage && filteredOffers.length > 0 && (
        <div className="offer-list">
          {filteredOffers.map((offer) => {
            const relatedOrder = getRelatedOrder(orders, offer.id)
            const isConverted = relatedOrder !== undefined

            const statusText = isConverted
              ? `${getOfferStatusLabel(offer.status)} · Auftrag erstellt`
              : getOfferStatusLabel(offer.status)

            return (
              <article key={offer.id} className="offer-card">
                <div className="offer-card__header">
                  <div>
                    <h3>{offer.offerNumber}</h3>
                    <p className="muted-text">{offer.customerName}</p>
                  </div>
                </div>

                <dl className="offer-card__details">
                  <dt>Status</dt>
                  <dd>{statusText}</dd>

                  <dt>Gültig bis</dt>
                  <dd>{formatDate(offer.validUntil)}</dd>

                  <dt>Gesamt netto</dt>
                  <dd>{formatCurrency(offer.totalNet)}</dd>
                </dl>

                {offer.notes && <p className="muted-text">{offer.notes}</p>}

                <div className="offer-card__actions">
                  <Link
                    to={`/offers/${offer.id}`}
                    className="secondary-link-button offer-card__action-button"
                  >
                    Angebot öffnen
                  </Link>

                  {relatedOrder && (
                    <Link
                      to={`/orders/${relatedOrder.id}`}
                      className="primary-button offer-card__action-button"
                    >
                      Auftrag öffnen
                    </Link>
                  )}
                </div>
              </article>
            )
          })}
        </div>
      )}
    </section>
  )
}