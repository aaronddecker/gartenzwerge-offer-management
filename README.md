# Gartenzwerge Außenservice – Angebots- und Auftragsmanagement

A modern full-stack business management application for managing customers, offered services, offers and service orders for a landscaping and outdoor service business.

The application is designed to support real business workflows such as customer management, offer creation, service pricing, order handling, authentication, authorization and a mobile-first frontend experience.

---

## Current Status

🚧 Active Development

Current milestone:

```text
v0.10.0 – Authentication UI & Protected Frontend
```

The backend already provides the core business API, authentication, JWT-based authorization and role-based endpoint protection.

The frontend foundation has been extended with a working authentication flow. The React client can log in through the backend API, store a JWT token, protect internal routes, load the current authenticated user through `/api/auth/me`, display the user's role and restrict Admin-only frontend areas.

---

## Completed

### Backend Foundation

* Customer CRUD API
* Offered Service CRUD API
* Offer CRUD API
* Offer Item Management

  * Add offer items
  * Get offer items by offer
  * Update offer item quantities
  * Soft delete offer items
  * Automatic item total calculation
  * Automatic offer total recalculation
* Order Management Foundation

  * Create orders from accepted offers
  * Prevent duplicate orders
  * View and update orders
  * Soft delete orders
* PostgreSQL integration
* Entity Framework Core persistence
* Repository Pattern
* Service Layer
* FluentValidation request validation
* Global exception handling
* Serilog logging
* Unit tests
* Swagger/OpenAPI documentation
* Docker Compose setup for local PostgreSQL

### Authentication and Authorization

* User registration with ASP.NET Core Identity
* User login with password validation
* Secure password hashing through ASP.NET Core Identity
* JWT token generation
* JWT bearer authentication setup
* Protected `/api/auth/me` endpoint
* Swagger JWT authorization support
* Protected business endpoints with JWT authentication
* Admin and Employee roles
* Role seeding for local development
* Development users for Admin and Employee authorization testing
* Role claims in JWT tokens
* Role-based endpoint protection
* Admin-only protection for critical delete and service management actions

### Frontend Foundation

* React + TypeScript + Vite frontend setup
* Frontend routing with React Router
* Basic app layout with shared navigation
* Mobile-first navigation structure
* More page for secondary navigation areas
* Analytics page placeholder
* Placeholder pages for dashboard, customers, offers, orders, offered services and login
* Reusable `PageHeader` component
* Reusable `StatCard` component
* Reusable `QuickActionLink` component
* Initial dashboard statistic cards
* Dashboard quick actions
* Mobile navigation improved to avoid horizontal scrolling

### Authentication UI & Protected Frontend

* Login form connected to the backend authentication API
* Loading, success and error states for the login flow
* Local CORS support for the React development frontend
* JWT token storage in the frontend
* Redirect to dashboard after successful login
* Logout action with token removal
* Protected frontend routes for authenticated users
* Public-only login route for already authenticated users
* Current user loading through `/api/auth/me`
* Current user email and role display in the app header
* Role-aware UI behavior for Admin and Employee users
* Admin-only frontend routes for Analytics and Offered Services
* `.vite/` ignored to prevent committing generated Vite cache files

---

## Planned

### Backend

* Full Order Management

  * Dedicated complete/cancel order endpoints
  * Order scheduling
  * Employee/user assignment
* Advanced Pricing Calculator
* AI-assisted offer creation

### Frontend

* Global AuthContext for cleaner shared authentication state
* Admin/Employee UI polishing
* Dashboard with upcoming orders
* Simple calendar field for upcoming orders
* Customer CRUD UI
* Offered Service CRUD UI
* Offer Management UI
* Order Management UI
* Loading, error and empty states for business data
* Responsive polishing

### DevOps

* Dockerized full-stack setup
* GitHub Actions CI pipeline
* Deployment

---

## Features

### Customer Management

* Create customers
* View customers
* Update customers
* Soft delete customers

### Offered Service Management

* Create offered services
* View offered services
* Update offered services
* Soft delete offered services
* Manage prices and units

### Offer Management

* Create offers for existing customers
* View offers
* Update offer metadata and status
* Soft delete offers
* Automatic offer number generation

### Offer Item Management

* Add offer items to existing offers
* View offer items by offer
* Update offer item quantities
* Soft delete offer items
* Link offer items to offered services
* Calculate item totals based on quantity and unit price
* Recalculate offer totals from all active offer items

### Order Management

* Create an order from an accepted offer
* Prevent creating orders from non-accepted offers
* Prevent duplicate orders for the same offer
* View all orders
* View a single order by id
* Update order status, planned date and notes
* Soft delete orders
* Automatically set `completedAt` when an order is completed
* Clear `completedAt` when an order is reopened

### Authentication

* Register users
* Log in users
* Generate JWT tokens
* Validate JWT bearer tokens
* Retrieve the currently authenticated user through `/api/auth/me`

### Authorization

* Admin and Employee roles
* Role claims in JWT tokens
* Role-based endpoint protection
* Admin-only access for critical actions
* `401 Unauthorized` for missing or invalid tokens
* `403 Forbidden` for insufficient role permissions

### Frontend Authentication and Route Protection

* Login form connected to the backend API
* JWT token storage in `localStorage`
* Redirect to dashboard after successful login
* Logout with token removal
* Protected routes for authenticated users
* Public-only login route for authenticated users
* Current user loading through `/api/auth/me`
* Current user email and role display in the header
* Role-aware UI behavior for Admin and Employee users
* Admin-only frontend route protection for Analytics and Offered Services

### Frontend UI

* Mobile-first layout
* App layout with shared navigation
* Dashboard foundation
* Quick action cards focused on the offer workflow
* More page for secondary and Admin-only areas
* Analytics area prepared for future Admin reporting
* Placeholder pages for core business modules

---

## Architecture

The backend follows Clean Architecture principles.

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
Application
 ↓
Domain
```

### Domain Layer

Contains the core business entities and domain concepts.

Examples:

* Customer
* OfferedService
* Offer
* OfferItem
* Order
* Enums
* BaseEntity

### Application Layer

Contains application-specific business logic and use cases.

Examples:

* DTOs
* Validators
* Service interfaces
* Application services
* Repository interfaces
* Application exceptions
* Authorization constants

### Infrastructure Layer

Contains technical implementations and external dependencies.

Examples:

* Entity Framework Core
* PostgreSQL persistence
* Repository implementations
* Database migrations
* ASP.NET Core Identity
* JWT token generation
* Role seeding

### API Layer

Exposes the application through REST endpoints.

Examples:

* Controllers
* Middleware
* Swagger configuration
* Dependency injection setup
* Authentication and authorization configuration

### Frontend

The frontend is a separate React client.

```text
React Frontend
 ↓ HTTP
ASP.NET Core API
 ↓
Application / Infrastructure / Database
```

This keeps the backend client-independent. A future mobile app could use the same API.

---

## Tech Stack

### Backend

* C#
* ASP.NET Core 9
* Entity Framework Core
* PostgreSQL
* ASP.NET Core Identity
* JWT Bearer Authentication
* FluentValidation
* Serilog
* Swagger / OpenAPI

### Frontend

* React
* TypeScript
* Vite
* React Router
* CSS with mobile-first responsive styling

### Testing

* xUnit
* FluentAssertions
* ESLint
* Frontend production build check

### Planned DevOps

* Docker
* Docker Compose
* GitHub Actions
* Azure App Service
* Azure Database for PostgreSQL

---

## API Endpoints

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
```

`/api/auth/me` requires a valid JWT bearer token and returns the current authenticated user including role information.

Example response:

```json
{
  "userId": "019eca0b-1a65-70bc-9821-1f6a0c344c11",
  "email": "test@gartenzwerge.de",
  "displayName": "Test User",
  "roles": ["Admin"]
}
```

### Customers

```http
GET    /api/customers
GET    /api/customers/{id}
POST   /api/customers
PUT    /api/customers/{id}
DELETE /api/customers/{id}
```

### Offered Services

```http
GET    /api/offered-services
GET    /api/offered-services/{id}
POST   /api/offered-services
PUT    /api/offered-services/{id}
DELETE /api/offered-services/{id}
```

### Offers

```http
GET    /api/offers
GET    /api/offers/{id}
POST   /api/offers
PUT    /api/offers/{id}
DELETE /api/offers/{id}
```

### Offer Items

```http
POST   /api/offers/{offerId}/items
GET    /api/offers/{offerId}/items
PUT    /api/offers/{offerId}/items/{itemId}
DELETE /api/offers/{offerId}/items/{itemId}
```

### Orders

```http
GET    /api/orders
GET    /api/orders/{id}
POST   /api/offers/{offerId}/order
PUT    /api/orders/{id}
DELETE /api/orders/{id}
```

---

## Frontend Routes

Current frontend routes:

```text
/dashboard
/customers
/offers
/orders
/more
/analytics
/offered-services
/login
```

Current frontend route behavior:

* `/login` is public and redirects authenticated users to `/dashboard`
* Internal app routes are protected for authenticated users
* `/analytics` is restricted to Admin users
* `/offered-services` is restricted to Admin users
* Unauthorized users are redirected to `/dashboard` for Admin-only frontend routes

Current frontend limitations:

* No global AuthContext yet
* No refresh token flow yet
* No real dashboard or analytics data yet
* No backend API integration for business data yet
* No Customer, Offer, Order or Offered Service CRUD UI yet

---

## Example Customer Request

```json
{
  "firstName": "Max",
  "lastName": "Mustermann",
  "company": null,
  "phoneNumber": "07123 456789",
  "email": "max.mustermann@example.com",
  "street": "Hauptstraße",
  "houseNumber": "12",
  "postalCode": "71735",
  "city": "Eberdingen",
  "notes": "Test customer"
}
```

## Example Offer Request

```json
{
  "customerId": "00000000-0000-0000-0000-000000000000",
  "validUntil": "2026-12-31T00:00:00Z",
  "notes": "Test offer for garden maintenance."
}
```

## Example Offer Item Request

```json
{
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "quantity": 10
}
```

---

## Running the Backend Locally

### Prerequisites

* .NET 9 SDK
* Docker Desktop
* PostgreSQL container via Docker Compose

### Start PostgreSQL

```bash
docker compose up -d
```

### Apply Database Migrations

```bash
cd src/Backend

dotnet ef database update --project Gartenzwerge.Infrastructure --startup-project Gartenzwerge.API
```

### Run the API

```bash
dotnet run --project Gartenzwerge.API
```

Swagger is available at:

```text
http://localhost:5041/swagger
```

---

## Running the Frontend Locally

### Prerequisites

* Node.js
* npm

### Install dependencies

```bash
cd src/Frontend

npm install
```

### Run the frontend

```bash
npm run dev
```

The frontend is available at:

```text
http://localhost:5173
```

### Build the frontend

```bash
npm run build
```

### Run ESLint

```bash
npm run lint
```

---

## Running Tests and Checks

### Backend

```bash
cd src/Backend

dotnet test
```

### Frontend

```bash
cd src/Frontend

npm run build
npm run lint
```

---

## Project Structure

```text
gartenzwerge-management/
├── docs/
│   ├── api/
│   ├── architecture/
│   ├── business-processes/
│   ├── database/
│   └── frontend/
├── src/
│   ├── Backend/
│   │   ├── Gartenzwerge.Domain/
│   │   ├── Gartenzwerge.Application/
│   │   ├── Gartenzwerge.Infrastructure/
│   │   └── Gartenzwerge.API/
│   └── Frontend/
│       ├── src/
│       │   ├── api/
│       │   ├── app/
│       │   ├── auth/
│       │   ├── pages/
│       │   └── shared/
│       ├── package.json
│       └── vite.config.ts
├── tests/
│   └── Gartenzwerge.UnitTests/
├── docker-compose.yml
├── README.md
└── .gitignore
```

---

## Documentation

Additional project documentation is available in the `docs` folder.

* [API Endpoints](docs/api/endpoints.md)
  Documents the available REST API endpoints, request bodies, response codes and example payloads.

* [Business Process Documentation](docs/business-processes/)
  Contains documentation about important business workflows, such as adding offer items and creating orders from accepted offers.

* [Architecture Documentation](docs/architecture/)
  Contains documentation about Clean Architecture, request flow, authentication and architectural decisions.

* [Database Documentation](docs/database/)
  Contains documentation about entities, relationships and database-related concepts.

* [Frontend Architecture](docs/frontend/frontend-architecture.md)
  Documents the React frontend structure, routing, mobile-first navigation, authentication flow and current frontend limitations.

---

## Development Workflow

This project follows a small-step development workflow.

Before committing changes:

* Make sure the backend builds successfully when backend code changed
* Run backend tests when backend code changed
* Make sure the frontend builds successfully when frontend code changed
* Run frontend linting when frontend code changed
* Test changed API endpoints through Swagger when applicable
* Test changed frontend routes in the browser when applicable
* Keep commits small and focused

### Commit Message Convention

Commit messages follow the Conventional Commits style:

```text
feat: add new user-visible functionality
fix: fix a bug or incorrect behavior
refactor: improve internal code structure without changing behavior
docs: update documentation
test: add or update tests
chore: update tooling, configuration or maintenance tasks
```

---

## Development Roadmap

### v0.1.0 – Customer Management Foundation

* Customer CRUD API
* Validation
* Error handling
* Logging
* Unit tests

### v0.2.0 – Service Management

* Manage offered services
* Service prices
* Service units
* Service CRUD API

### v0.3.0 – Offer Management

* Create offers
* Manage offer status
* Automatic offer number generation
* Unit tests

### v0.4.0 – Offer Item Management

* Add offer items to existing offers
* Calculate offer item totals
* Recalculate offer totals
* Connect offer items to offered services

### v0.5.0 – Request Validation

* Lawn mowing price tiers
* Hedge cutting price calculation
* Green waste disposal calculation
* Travel cost calculation

### v0.6.0 – Order Management Foundation

* Convert accepted offers into orders
* Prevent duplicate orders
* Add order validation
* Unit tests

### v0.7.0 – Authentication and Protected API Foundation

* User registration with ASP.NET Core Identity
* User login with password validation
* Secure password hashing
* JWT token generation
* JWT bearer authentication setup
* Protected `/api/auth/me` endpoint
* Swagger JWT authorization support
* Auth request validation
* Unit tests
* Protected business endpoints with JWT authentication

### v0.8.0 – Authorization & User Roles

* User roles such as Admin and Employee
* Role seeding for local development
* Role claims in JWT tokens
* Role-based endpoint protection
* Admin-only protection for critical actions

### v0.9.0 – Frontend Foundation

* React + TypeScript frontend setup
* Vite project structure
* Routing
* Basic app layout
* Mobile-first navigation
* Placeholder pages for core business areas
* More and Analytics pages
* Reusable UI components
* Initial dashboard foundation
* Dashboard quick actions

### v0.10.0 – Authentication UI & Protected Frontend

* Login form connected to the backend authentication API
* JWT token handling in the frontend
* Redirect after successful login
* Logout action
* Protected frontend routes
* Public-only login route
* Load current user through `/api/auth/me`
* Display current user email and role in the header
* Basic role-aware frontend UI
* Admin-only frontend routes for Analytics and Offered Services

### v0.11.0 – Customer and Service UI

* Customer management UI
* Offered service management UI
* Form validation
* API integration
* Loading, error and empty states

### v0.12.0 – Offer Management UI

* Offer overview
* Offer detail view
* Offer item management
* Offer total display
* Smart customer lookup during offer creation

### v0.13.0 – Order Management UI

* Order overview
* Order detail view
* Update order status
* Display completed orders
* Dashboard upcoming orders
* Simple calendar field for upcoming orders

### v0.14.0 – Analytics & Reporting UI

* Customer statistics
* Completed order statistics
* Revenue overview
* Revenue chart
* Offer-to-order conversion insights

### v0.15.0 – Fullstack Business Workflow MVP

* End-to-end workflow from customer to offer to order
* Backend and frontend connected
* Local full-stack setup documented

### v0.16.0 – CI Pipeline

* GitHub Actions workflow
* Automated build
* Automated tests on pull requests

### v1.0.0 – Full Business Workflow MVP

* Authentication and authorization
* Frontend for core business workflows
* Backend and frontend working together
* Dockerized local setup
* CI pipeline
* Complete README setup instructions
* Stable portfolio-ready MVP

---

## Future Ideas

* Customer portal with customer-specific login
* Automatic customer lookup during offer creation
* Create a new customer directly inside the offer creation form if no matching customer exists
* PDF generation for offers
* Email sending for offers
* PWA support
* Native mobile client using the same backend API

---

## Author

Aaron Decker
