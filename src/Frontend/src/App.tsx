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
import { OfferCreatePage } from './pages/OfferCreatePage'
import { PublicOnlyRoute } from './auth/PublicOnlyRoute'
import { RoleProtectedRoute } from './auth/RoleProtectedRoute'
import './App.css'

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />

        <Route element={<PublicOnlyRoute />}>
          <Route path="/login" element={<LoginPage />} />
        </Route>

        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/customers" element={<CustomersPage />} />
            <Route path="/offers" element={<OffersPage />} />
            <Route path="/offers/new" element={<OfferCreatePage />} />
            <Route path="/orders" element={<OrdersPage />} />
            <Route path="/more" element={<MorePage />} />
            

            <Route element={<RoleProtectedRoute requiredRole="Admin" />}>
              <Route path="/analytics" element={<AnalyticsPage />} />
              <Route path="/offered-services" element={<OfferedServicesPage />} />
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
