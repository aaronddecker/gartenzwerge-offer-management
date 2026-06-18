import { Link } from 'react-router-dom'
import { PageHeader } from '../shared/components/PageHeader'

export function MorePage() {
  return (
    <section className="page">
      <PageHeader
        title="Mehr"
        description="Weitere Bereiche und Verwaltungsfunktionen."
      />

      <div className="more-link-list">
        <Link to="/analytics" className="more-link">
          Analytics
        </Link>
        <Link to="/offered-services" className="more-link">
          Leistungen verwalten
        </Link>
      </div>
    </section>
  )
}