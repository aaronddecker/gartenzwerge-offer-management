import { Navigate, Outlet } from 'react-router-dom'
import { hasAuthToken } from './authStorage'

export function PublicOnlyRoute() {
  if (hasAuthToken()) {
    return <Navigate to="/dashboard" replace />
  }

  return <Outlet />
}