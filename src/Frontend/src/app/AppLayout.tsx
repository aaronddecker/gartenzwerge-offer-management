import { NavLink, Outlet } from 'react-router-dom'

export function AppLayout() {
  return (
    <div className="app-layout">
      <header className="app-header">
        <div>
          <p className="app-kicker">Gartenzwerge Außenservice</p>
          <h1>Management</h1>
        </div>
      </header>

      <nav className="app-nav" aria-label="Main navigation">
        <NavLink to="/dashboard">Dashboard</NavLink>
        <NavLink to="/customers">Kunden</NavLink>
        <NavLink to="/offered-services">Leistungen</NavLink>
        <NavLink to="/offers">Angebote</NavLink>
        <NavLink to="/orders">Aufträge</NavLink>
      </nav>

      <main className="app-content">
        <Outlet />
      </main>
    </div>
  )
}