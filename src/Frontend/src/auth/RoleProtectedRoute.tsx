import { Navigate, Outlet, useOutletContext } from 'react-router-dom'
import type { CurrentUserResponse } from '../api/authApi'

type AppLayoutOutletContext = {
  currentUser: CurrentUserResponse | null
}

type RoleProtectedRouteProps = {
  requiredRole: string
}

export function RoleProtectedRoute({ requiredRole }: RoleProtectedRouteProps) {
  const { currentUser } = useOutletContext<AppLayoutOutletContext>()

  if (currentUser === null) {
    return (
      <section className="page">
        <p className="muted-text">Berechtigung wird geprüft...</p>
      </section>
    )
  }

  const hasRequiredRole = currentUser.roles?.includes(requiredRole) ?? false

  if (!hasRequiredRole) {
    return <Navigate to="/dashboard" replace />
  }

  return <Outlet />
}