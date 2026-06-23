import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getOffers,
  type OfferResponse,
  type OfferStatus,
} from '../api/offersApi'
import { PageHeader } from '../shared/components/PageHeader'

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
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    getOffers()
      .then((loadedOffers) => {
        if (!isMounted) {
          return
        }

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
        title="Angebote"
        description="Erstelle und verwalte Angebote für bestehende und neue Kunden."
      />

      <div className="page-actions">
        <Link to="/offers/new" className="primary-link-button">
          Neues Angebot
        </Link>
      </div>

      {isLoading && <p className="muted-text">Angebote werden geladen...</p>}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && offers.length === 0 && (
        <p className="muted-text">Es sind noch keine Angebote vorhanden.</p>
      )}

      {!isLoading && !errorMessage && offers.length > 0 && (
        <div className="offer-list">
          {offers.map((offer) => (
            <article key={offer.id} className="offer-card">
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
            </article>
          ))}
        </div>
      )}
    </section>
  )
}