import { PageHeader } from '../shared/components/PageHeader'

export function DashboardPage() {
  return (
    <section className="page">
      <PageHeader
        title="Dashboard"
        description="Übersicht über Kunden, Angebote und Aufträge."
      />
    </section>
  )
}