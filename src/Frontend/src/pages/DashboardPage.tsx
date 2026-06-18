import { PageHeader } from '../shared/components/PageHeader'
import { QuickActionLink } from '../shared/components/QuickActionLink'
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

      <section className="dashboard-section">
        <h3>Schnellaktionen</h3>

        <div className="quick-action-grid">
          <QuickActionLink
            to="/customers"
            title="Kunden verwalten"
            description="Kunden anzeigen, anlegen oder bearbeiten"
          />
          <QuickActionLink
            to="/offers"
            title="Angebote bearbeiten"
            description="Angebote prüfen und neue Angebote vorbereiten"
          />
          <QuickActionLink
            to="/orders"
            title="Aufträge anzeigen"
            description="Geplante und laufende Aufträge im Blick behalten"
          />
          <QuickActionLink
            to="/offered-services"
            title="Leistungen verwalten"
            description="Dienstleistungen, Preise und Einheiten pflegen"
          />
        </div>
      </section>
    </section>
  )
}