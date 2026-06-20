# Frontend Architecture

This document explains the current frontend foundation and authentication flow of the Gartenzwerge management application.

## Goal

The frontend is built as a React client for the existing ASP.NET Core backend API.

It provides the foundation for a mobile-first business application with routing, a shared app layout, reusable UI components, authentication, protected routes and role-aware frontend behavior.

The backend remains the actual security boundary. Frontend route protection improves user experience and prevents users from navigating into areas that are not relevant for their role.

## Technologies

* React
* TypeScript
* Vite
* React Router
* CSS with mobile-first responsive styling
* Browser `localStorage` for the current development JWT storage

## Project Structure

```text
src/
├── api/
├── app/
├── auth/
├── pages/
└── shared/
```

## Responsibilities

### app

Contains app-wide structure such as layout and routing-related components.

Examples:

* `AppLayout`

### api

Contains frontend API functions for communication with the ASP.NET Core backend.

Current examples:

* `authApi`
  * `login`
  * `getCurrentUser`

This area can later be expanded with endpoint-specific API functions for customers, offered services, offers and orders.

### auth

Contains authentication-related frontend helpers and route guard components.

Examples:

* `authStorage`
* `ProtectedRoute`
* `PublicOnlyRoute`
* `RoleProtectedRoute`

Responsibilities:

* Store the JWT token
* Read the JWT token
* Remove the JWT token on logout
* Protect authenticated frontend routes
* Redirect authenticated users away from public-only routes such as `/login`
* Protect Admin-only frontend routes

### pages

Contains route-level page components.

Examples:

* `DashboardPage`
* `CustomersPage`
* `OffersPage`
* `OrdersPage`
* `AnalyticsPage`
* `MorePage`
* `OfferedServicesPage`
* `LoginPage`

### shared

Contains reusable UI components that can be used across multiple pages.

Examples:

* `PageHeader`
* `StatCard`
* `QuickActionLink`

## Routing

Current frontend routes:

```text
/login
/dashboard
/customers
/offers
/orders
/more
/analytics
/offered-services
```

## Route Protection

The frontend currently uses three route guard components.

### ProtectedRoute

`ProtectedRoute` protects internal app routes for authenticated users.

If no token is available, the user is redirected to `/login`.

Protected routes include:

```text
/dashboard
/customers
/offers
/orders
/more
```

### PublicOnlyRoute

`PublicOnlyRoute` prevents already authenticated users from opening the login page.

If a token is already available, `/login` redirects to `/dashboard`.

### RoleProtectedRoute

`RoleProtectedRoute` protects Admin-only frontend routes.

Current Admin-only routes:

```text
/analytics
/offered-services
```

If an Employee opens one of these routes directly, the user is redirected to `/dashboard`.

This improves the frontend user flow. The backend still remains responsible for enforcing real authorization.

## Authentication Flow

The current frontend authentication flow works as follows:

```text
LoginPage
→ POST /api/auth/login
→ JWT token received
→ token stored in localStorage
→ redirect to /dashboard
→ AppLayout calls GET /api/auth/me
→ backend validates the token
→ frontend receives current user data
→ email and role are displayed in the app header
```

## Logout Flow

The logout flow works as follows:

```text
Logout button
→ remove token from localStorage
→ redirect to /login
```

## Current User Loading

The frontend loads the current authenticated user through:

```http
GET /api/auth/me
```

Expected response example:

```json
{
  "userId": "019eca0b-1a65-70bc-9821-1f6a0c344c11",
  "email": "test@gartenzwerge.de",
  "displayName": "Test User",
  "roles": ["Admin"]
}
```

The user information is shown in the app header.

Example:

```text
test@gartenzwerge.de · Admin
```

## Role-Based Frontend Behavior

The frontend currently distinguishes between Admin and Employee users.

### Admin

Admins can:

* See Analytics in the More page
* Access `/analytics`
* See Offered Services in the More page
* Access `/offered-services`

### Employee

Employees can:

* Access regular authenticated app areas
* Use Dashboard, Customers, Offers and Orders pages

Employees cannot:

* See Analytics in the More page
* Access `/analytics`
* See Offered Services in the More page
* Access `/offered-services`

If an Employee opens an Admin-only route directly, the frontend redirects to `/dashboard`.

## UI/UX Decisions

The frontend follows a mobile-first approach.

On mobile, the primary navigation focuses on the most important work areas:

```text
Dashboard
Customers
Offers
Orders
More
```

Secondary and Admin-specific areas such as Analytics and Offered Services are accessible through the More page for Admin users.

This keeps the main navigation simple and avoids horizontal scrolling on mobile devices.

## Mobile Navigation

The mobile navigation is designed as a bottom navigation.

Reasons:

* Important areas are directly reachable
* No hidden horizontal scrolling
* Better app-like user experience on mobile
* Clear structure for future mobile or PWA usage

The navigation uses safe-area spacing so that browser or device controls do not cover the menu on mobile devices.

## Dashboard vs Analytics

The dashboard is intended as an operational entry point.

It should focus on:

* The most important next action
* Upcoming work
* Important daily overview
* Fast access to core workflows

The dashboard quick action currently focuses on the offer workflow.

Analytics is intended for reporting and business insights.

It can later include:

* Customer statistics
* Completed order statistics
* Revenue overview
* Revenue charts
* Offer-to-order conversion insights
* Top services

Analytics is currently treated as an Admin-only area.

## Current Frontend Status

Implemented:

* React + TypeScript + Vite setup
* React Router foundation
* Basic app layout
* Mobile-first navigation
* Safe-area-aware bottom navigation on mobile
* More page
* Analytics page placeholder
* Placeholder pages for core business areas
* Reusable page header component
* Reusable statistic card component
* Reusable quick action link component
* Initial dashboard foundation
* Simplified dashboard quick action focused on offers
* Login form UI
* Login API integration through `POST /api/auth/login`
* Login loading, success and error states
* JWT token storage
* Redirect after successful login
* Logout action
* Protected frontend routes
* Public-only login route
* Current user loading through `/api/auth/me`
* Current user email and role display in the header
* Role-aware UI behavior
* Admin-only frontend routes for Analytics and Offered Services

## Current Limitations

* No global AuthContext yet
* No refresh token handling yet
* No automatic token expiration handling in the frontend yet
* No full API client abstraction for all business endpoints yet
* No real dashboard data yet
* No real analytics data yet
* Customer, service, offer and order pages are still placeholders
* No customer CRUD UI yet
* No offered service CRUD UI yet
* No offer management UI yet
* No order management UI yet

## Future Improvements

Planned frontend improvements:

* Global auth state handling
* Cleaner shared authorization helpers
* Better unauthorized-state UX
* Optional dedicated access denied page
* Token expiration handling
* Customer CRUD UI
* Offered Service CRUD UI
* Offer Management UI
* Smart customer lookup during offer creation
* Order Management UI
* Dashboard with upcoming orders
* Calendar field for upcoming orders
* Analytics with real business data
* Loading, error and empty states for business pages
* API client abstraction for business endpoints
* PWA support
