import { Navigate, Outlet } from 'react-router-dom'
import { hasAuthToken } from './authStorage'

export function ProtectedRoute() {
  if (!hasAuthToken()) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}