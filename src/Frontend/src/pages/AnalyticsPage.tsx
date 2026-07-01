import { useEffect, useState } from 'react'
import { getOffers, type OfferResponse } from '../api/offersApi'
import { getOrders, type OrderResponse } from '../api/ordersApi'
import { AnimatedNumber } from '../shared/components/AnimatedNumber'
import { PageHeader } from '../shared/components/PageHeader'
import { PageState } from '../shared/components/PageState'
import { TrendBarChart, type ChartPoint } from '../shared/components/TrendBarChart'
import { getRelatedOrder, isOpenOffer } from '../shared/businessRules'

const MONTHS_TO_SHOW = 12

function formatCurrency(value: number) {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(value)
}

function formatAxisCurrency(value: number) {
  return `${new Intl.NumberFormat('de-DE', {
    maximumFractionDigits: 0,
  }).format(value)} €`
}

function formatPercent(value: number) {
  return `${Math.round(value)} %`
}

function getOfferForOrder(offers: OfferResponse[], order: OrderResponse) {
  return offers.find((offer) => offer.id === order.offerId)
}

// Sums the related offer value over the given orders.
function sumOrderVolume(orders: OrderResponse[], offers: OfferResponse[]) {
  return orders.reduce((total, order) => {
    const relatedOffer = getOfferForOrder(offers, order)
    return total + (relatedOffer?.totalNet ?? 0)
  }, 0)
}

// Builds one revenue bucket per month for the last MONTHS_TO_SHOW months,
// summing the value of orders completed within each month.
function getMonthlyRevenue(
  completedOrders: OrderResponse[],
  offers: OfferResponse[]
): ChartPoint[] {
  const now = new Date()

  const buckets = Array.from({ length: MONTHS_TO_SHOW }, (_, position) => {
    const monthsAgo = MONTHS_TO_SHOW - 1 - position
    const date = new Date(now.getFullYear(), now.getMonth() - monthsAgo, 1)

    return {
      key: date.getFullYear() * 12 + date.getMonth(),
      label: new Intl.DateTimeFormat('de-DE', { month: 'short' }).format(date),
      value: 0,
    }
  })

  const indexByKey = new Map(buckets.map((bucket, index) => [bucket.key, index]))

  for (const order of completedOrders) {
    if (!order.completedAt) {
      continue
    }

    const completedDate = new Date(order.completedAt)
    const key = completedDate.getFullYear() * 12 + completedDate.getMonth()
    const index = indexByKey.get(key)

    if (index === undefined) {
      continue
    }

    const relatedOffer = getOfferForOrder(offers, order)
    buckets[index].value += relatedOffer?.totalNet ?? 0
  }

  return buckets.map((bucket) => ({ label: bucket.label, value: bucket.value }))
}

export function AnalyticsPage() {
  const [offers, setOffers] = useState<OfferResponse[]>([])
  const [orders, setOrders] = useState<OrderResponse[]>([])
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

  // Auftragsvolumen: all orders except cancelled.
  const billableOrders = orders.filter((order) => order.status !== 4)
  const orderVolume = sumOrderVolume(billableOrders, offers)

  // Completed orders drive the realised volume and the monthly chart.
  const completedOrders = orders.filter((order) => order.status === 3)
  const completedVolume = sumOrderVolume(completedOrders, offers)

  // Vertrieb & Conversion.
  const conversionRate =
    offers.length === 0
      ? 0
      : Math.min(100, (orders.length / offers.length) * 100)
  const averageOrderValue =
    billableOrders.length === 0 ? 0 : orderVolume / billableOrders.length
  const pipelineValue = offers
    .filter((offer) => isOpenOffer(offer, getRelatedOrder(orders, offer.id)))
    .reduce((total, offer) => total + offer.totalNet, 0)

  const monthlyRevenue = getMonthlyRevenue(completedOrders, offers)

  return (
    <section className="page">
      <PageHeader
        title="Analytics"
        description="Auswertungen zu Aufträgen, Angeboten und Umsatz."
      />

      <PageState
        isLoading={isLoading}
        loadingText="Auswertungen werden geladen..."
        error={errorMessage}
      >
          <section className="analytics-section">
            <h3>Umsatz</h3>
            <div className="report-grid">
              <article className="report-card">
                <p className="report-card__title">Auftragsvolumen</p>
                <strong className="report-card__value">
                  <AnimatedNumber value={orderVolume} format={formatCurrency} />
                </strong>
                <p className="report-card__description">
                  Summe aller Aufträge ohne stornierte
                </p>
              </article>

              <article className="report-card">
                <p className="report-card__title">Abgeschlossenes Volumen</p>
                <strong className="report-card__value">
                  <AnimatedNumber
                    value={completedVolume}
                    format={formatCurrency}
                  />
                </strong>
                <p className="report-card__description">
                  Umsatz aus abgeschlossenen Aufträgen
                </p>
              </article>
            </div>
          </section>

          <section className="analytics-section">
            <h3>Umsatzentwicklung (12 Monate)</h3>
            <TrendBarChart
              data={monthlyRevenue}
              formatValue={formatCurrency}
              formatAxis={formatAxisCurrency}
            />
          </section>

          <section className="analytics-section">
            <h3>Vertrieb &amp; Conversion</h3>
            <div className="report-grid">
              <article className="report-card">
                <p className="report-card__title">Conversion-Rate</p>
                <strong className="report-card__value">
                  <AnimatedNumber value={conversionRate} format={formatPercent} />
                </strong>
                <p className="report-card__description">
                  Angebote, die zu Aufträgen wurden
                </p>
              </article>

              <article className="report-card">
                <p className="report-card__title">Ø Auftragswert</p>
                <strong className="report-card__value">
                  <AnimatedNumber
                    value={averageOrderValue}
                    format={formatCurrency}
                  />
                </strong>
                <p className="report-card__description">
                  Durchschnitt je Auftrag ohne stornierte
                </p>
              </article>

              <article className="report-card">
                <p className="report-card__title">Angebots-Pipeline</p>
                <strong className="report-card__value">
                  <AnimatedNumber value={pipelineValue} format={formatCurrency} />
                </strong>
                <p className="report-card__description">
                  Wert noch offener Angebote
                </p>
              </article>
            </div>
          </section>
      </PageState>
    </section>
  )
}
