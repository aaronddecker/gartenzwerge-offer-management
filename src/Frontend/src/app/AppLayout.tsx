import { useEffect, useState } from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { getCurrentUser, type CurrentUserResponse } from '../api/authApi'
import { removeAuthToken } from '../auth/authStorage'

export function AppLayout() {
  const navigate = useNavigate()
  const [currentUser, setCurrentUser] = useState<CurrentUserResponse | null>(null)

  useEffect(() => {
    let isMounted = true

    async function loadCurrentUser() {
      try {
        const user = await getCurrentUser()

        if (isMounted) {
          setCurrentUser(user)
        }
      } catch {
        removeAuthToken()
        navigate('/login', { replace: true })
      }
    }

    loadCurrentUser()

    return () => {
      isMounted = false
    }
  }, [navigate])

  function handleLogout() {
    removeAuthToken()
    navigate('/login', { replace: true })
  }

  function formatRoles(roles?: string[]) {
  if (!roles || roles.length === 0) {
    return 'Keine Rolle geladen'
  }

  return roles.join(', ')
}

  return (
    <div className="app-layout">
      <header className="app-header">
        <div className="app-header__title">
          <p className="app-kicker">Gartenzwerge Außenservice</p>
          <h1>Management</h1>
          <p className="app-current-user">
            {currentUser
              ? `${currentUser.email} · ${formatRoles(currentUser.roles)}`
              : 'Benutzer wird geladen...'}
          </p>
        </div>

        <button type="button" className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </header>

      <nav className="app-nav" aria-label="Main navigation">
        <NavLink to="/dashboard">Start</NavLink>
        <NavLink to="/customers">Kunden</NavLink>
        <NavLink to="/offers">Angebote</NavLink>
        <NavLink to="/orders">Aufträge</NavLink>
        <NavLink to="/more">Mehr</NavLink>
      </nav>

      <main className="app-content">
        <Outlet />
      </main>
    </div>
  )
}