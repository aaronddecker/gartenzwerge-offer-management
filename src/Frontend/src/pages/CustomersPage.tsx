import { useEffect, useState } from 'react'
import { getCustomers, type CustomerResponse } from '../api/customersApi'
import { PageHeader } from '../shared/components/PageHeader'

export function CustomersPage() {
  const [customers, setCustomers] = useState<CustomerResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    async function loadCustomers() {
      try {
        const loadedCustomers = await getCustomers()

        if (isMounted) {
          setCustomers(loadedCustomers)
          setErrorMessage(null)
        }
      } catch (error) {
        if (isMounted) {
          setErrorMessage(
            error instanceof Error
              ? error.message
              : 'Ein unbekannter Fehler ist aufgetreten.'
          )
        }
      } finally {
        if (isMounted) {
          setIsLoading(false)
        }
      }
    }

    loadCustomers()

    return () => {
      isMounted = false
    }
  }, [])

  return (
    <section className="page">
      <PageHeader
        title="Kunden"
        description="Verwalte Kundenstammdaten für Angebote und Aufträge."
      />

      {isLoading && (
        <p className="muted-text">Kunden werden geladen...</p>
      )}

      {!isLoading && errorMessage && (
        <p className="form-message form-message--error">{errorMessage}</p>
      )}

      {!isLoading && !errorMessage && customers.length === 0 && (
        <p className="muted-text">
          Es sind noch keine Kunden vorhanden.
        </p>
      )}

      {!isLoading && !errorMessage && customers.length > 0 && (
        <div className="customer-list">
          {customers.map((customer) => (
            <article key={customer.id} className="customer-card">
              <div>
                <h3>
                  {customer.company ||
                    `${customer.firstName} ${customer.lastName}`}
                </h3>

                {customer.company && (
                  <p className="muted-text">
                    {customer.firstName} {customer.lastName}
                  </p>
                )}
              </div>

              <dl className="customer-card__details">
                {customer.email && (
                  <>
                    <dt>E-Mail</dt>
                    <dd>{customer.email}</dd>
                  </>
                )}

                {customer.phoneNumber && (
                  <>
                    <dt>Telefon</dt>
                    <dd>{customer.phoneNumber}</dd>
                  </>
                )}

                {customer.city && (
                  <>
                    <dt>Ort</dt>
                    <dd>{customer.city}</dd>
                  </>
                )}
              </dl>
            </article>
          ))}
        </div>
      )}
    </section>
  )
}