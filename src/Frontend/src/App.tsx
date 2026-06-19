import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AppLayout } from './app/AppLayout'
import { ProtectedRoute } from './auth/ProtectedRoute'
import { AnalyticsPage } from './pages/AnalyticsPage'
import { CustomersPage } from './pages/CustomersPage'
import { DashboardPage } from './pages/DashboardPage'
import { LoginPage } from './pages/LoginPage'
import { MorePage } from './pages/MorePage'
import { OfferedServicesPage } from './pages/OfferedServicesPage'
import { OffersPage } from './pages/OffersPage'
import { OrdersPage } from './pages/OrdersPage'
import './App.css'

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/login" element={<LoginPage />} />

        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/customers" element={<CustomersPage />} />
            <Route path="/offers" element={<OffersPage />} />
            <Route path="/orders" element={<OrdersPage />} />
            <Route path="/more" element={<MorePage />} />
            <Route path="/analytics" element={<AnalyticsPage />} />
            <Route path="/offered-services" element={<OfferedServicesPage />} />
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  )
}