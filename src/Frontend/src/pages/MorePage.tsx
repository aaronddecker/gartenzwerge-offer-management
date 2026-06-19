import { Link, useOutletContext } from 'react-router-dom'
import { PageHeader } from '../shared/components/PageHeader'
import type { CurrentUserResponse } from '../api/authApi'

type AppLayoutOutletContext = {
  currentUser: CurrentUserResponse | null
}

function hasRole(user: CurrentUserResponse | null, role: string) {
  return user?.roles?.includes(role) ?? false
}

export function MorePage() {
  const { currentUser } = useOutletContext<AppLayoutOutletContext>()
  const isAdmin = hasRole(currentUser, 'Admin')

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

        {isAdmin && (
          <Link to="/offered-services" className="more-link">
            Leistungen verwalten
          </Link>
        )}
      </div>
    </section>
  )
}