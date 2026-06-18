import { PageHeader } from '../shared/components/PageHeader'
import { StatCard } from '../shared/components/StatCard'

export function DashboardPage() {
  return (
    <section className="page">
      <PageHeader
        title="Dashboard"
        description="Übersicht über Kunden, Angebote und Aufträge."
      />

      <div className="stat-grid">
        <StatCard
          title="Kunden"
          value="0"
          description="Aktuell angelegte Kunden"
        />
        <StatCard
          title="Offene Angebote"
          value="0"
          description="Angebote, die noch nicht abgeschlossen sind"
        />
        <StatCard
          title="Anstehende Aufträge"
          value="0"
          description="Geplante Aufträge mit Termin"
        />
      </div>
    </section>
  )
}