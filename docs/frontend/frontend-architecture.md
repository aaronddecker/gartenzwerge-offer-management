# Frontend Architecture

This document explains the current frontend foundation and authentication flow of the Gartenzwerge management application.

## Goal

The frontend is built as a React client for the existing ASP.NET Core backend API.

It provides a mobile-first business application with routing, a shared app layout, reusable UI components, authentication, protected routes, role-aware frontend behavior and connected business workflows.

The frontend currently supports customer management, offered service creation and the offer creation workflow.

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
├── shared/
└── styles/
	├── components/
	└── pages/
```

## Responsibilities

### app

Contains app-wide structure such as layout and routing-related components.

Examples:

* `AppLayout`

### api

Contains frontend API functions for communication with the ASP.NET Core backend.

Current examples:

- `authApi`
	- `login`
	- `getCurrentUser`
- `customersApi`
	- `getCustomers`
	- `createCustomer`
	- `updateCustomer`
	- `deleteCustomer`
- `offeredServicesApi`
	- `getOfferedServices`
	- `createOfferedService`
- `offersApi`
	- `getOffers`
	- `getOfferById`
	- `createOffer`
- `offerItemsApi`
	- `getOfferItems`
	- `createOfferItem`

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

- `DashboardPage`
- `CustomersPage`
- `OffersPage`
- `OfferCreatePage`
- `OfferDetailsPage`
- `OrdersPage`
- `AnalyticsPage`
- `MorePage`
- `OfferedServicesPage`
- `LoginPage`

Contains route-level page components.

### shared

Contains reusable UI components that can be used across multiple pages.

Examples:

* `PageHeader`
* `StatCard`
* `QuickActionLink`

### styles

Contains structured global CSS files.

Examples:

- `tokens.css`
- `base.css`
- `layout.css`
- `components/buttons.css`
- `components/forms.css`
- `pages/customers.css`
- `pages/offers.css`

## Routing

Current frontend routes:

```text
/login
/dashboard
/customers
/offers
/offers/new
/offers/:offerId
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
/offers/new
/offers/:offerId
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

## Offer Creation Workflow

The offer workflow is split across multiple pages.

### OffersPage

Route:

```text
/offers
````

Responsibilities:

* Load offers from the backend API
* Display offer cards
* Show offer number, customer name, status, valid-until date and total amount
* Navigate to the offer creation page
* Navigate to offer details

### OfferCreatePage

Route:

```text
/offers/new
```

Responsibilities:

* Load existing customers
* Search customers while typing
* Show matching customer suggestions
* Allow selecting an existing customer
* Automatically show new customer fields if no matching customer exists
* Create a customer first if needed
* Create the offer afterwards using the resolved customer id
* Redirect back to `/offers` after successful creation

The user never needs to know or enter a technical `customerId`. The UI shows customer names and customer details, while the frontend internally uses the selected or newly created customer id for the backend request.

### OfferDetailsPage

Route:

```text
/offers/:offerId
```

Responsibilities:

* Load offer details
* Load offer items
* Load active offered services
* Display offer summary
* Display existing offer items
* Add new offer items by selecting an offered service and entering a quantity
* Refresh offer details and total amount after adding offer items


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
* Customer management UI with read, create, update and Admin-only delete
* Offered Services UI with read and create
* Offer overview
* Offer creation page
* Customer lookup during offer creation
* New customer creation during offer creation
* Offer details page
* Offer item creation
* Structured frontend CSS files for tokens, layout, components and page-specific styles

## Current Limitations

* No global AuthContext yet
* No refresh token handling yet
* No automatic token expiration handling in the frontend yet
* No full API client abstraction yet
* No real dashboard data yet
* No real analytics data yet
* Orders are not yet connected to the frontend workflow
* Offer status transitions are not yet managed in the frontend
* Creating an order from an accepted offer is planned for a later milestone
* Offered services currently support read and create in the frontend
* Customer lookup during offer creation is currently performed client-side after loading all customers
* The Customers page still contains a visible customer creation form, although the main business workflow now also supports creating customers during offer creation

## Future Improvements

Planned frontend improvements:

* Global auth state handling
* Cleaner shared authorization helpers
* Better unauthorized-state UX
* Optional dedicated access denied page
* Token expiration handling
* Backend-supported customer search endpoint if customer volume grows
* Improve Customers page into a clearer master data management area
* Offered Service edit and delete UI
* Offer status management
* Create orders from accepted offers
* Order Management UI
* Dashboard with upcoming orders
* Calendar field for upcoming orders
* Analytics with real business data
* API client abstraction for business endpoints
* PWA support

