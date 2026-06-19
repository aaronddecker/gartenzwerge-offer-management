import { PageHeader } from '../shared/components/PageHeader'
import { QuickActionLink } from '../shared/components/QuickActionLink'
import { StatCard } from '../shared/components/StatCard'

export function DashboardPage() {
  return (
    <section className="page">
      <PageHeader
        title="Dashboard"
        description="Dein Überblick über Kunden, Angebote und anstehende Aufträge."
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
          description="Angebote, die noch nicht angenommen oder abgelehnt wurden"
        />
        <StatCard
          title="Anstehende Aufträge"
          value="0"
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
        <p className="muted-text">
          Hier werden später die nächsten geplanten Aufträge angezeigt.
        </p>
      </section>
    </section>
  )
}