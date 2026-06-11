# Gartenzwerge Außenservice – Angebots- und Auftragsmanagement

A modern full-stack business management application for managing customers, offers and service orders for a landscaping and outdoor service business.

The application is designed to support real business workflows such as customer management, offer creation, service pricing and order handling while demonstrating clean architecture and professional software engineering practices.

---

## Current Status

🚧 Active Development

### Completed
- Customer CRUD API
- Offered Service CRUD API
- Offer CRUD API
- Offer Item Management
	- Add offer items
	- Get offer items by offer
	- Update offer item quantities
	- Soft delete offer items
	- Automatic item total calculation
	- Automatic offer total recalculation
- Create orders from accepted offers
- Order Management Foundation
- Create orders from accepted offers
- View and update orders
- PostgreSQL integration
- Entity Framework Core persistence
- Repository Pattern
- Service Layer
- FluentValidation request validation
- Global exception handling
- Serilog logging
- Unit tests
- Swagger/OpenAPI documentation
- Docker Compose setup for local PostgreSQL

### Planned

- Full Order Management
	- Get orders
- Advanced Pricing Calculator
- Authentication & Authorization
- GitHub Actions CI/CD
- Azure Deployment
- Frontend with React and TypeScript
- OpenAI Integration

---

## Features

### Customer Management

* Create customers
* View customers
* Update customers
* Soft delete customers

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

### Validation

* Request validation using FluentValidation
* Required field validation
* Empty GUID validation
* Email format validation
* Quantity validation
* Price validation
* Future date validation
* Maximum length validation

### Error Handling

* Global exception middleware
* Standardized JSON error responses
* Trace identifier support for debugging

### Logging

* Structured logging with Serilog
* Console logging
* File-based rolling logs

### Testing

* Unit tests with xUnit
* FluentAssertions
* Testable service layer through repository abstractions

---

## Architecture

The application follows Clean Architecture principles.

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ↑
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
* Service interfaces
* Application services
* Repository interfaces
* Validators

### Infrastructure Layer

Contains technical implementations and external dependencies.

Examples:

* Entity Framework Core
* PostgreSQL persistence
* Repository implementations
* Database migrations

### API Layer

Exposes the application through REST endpoints.

Examples:

* Controllers
* Middleware
* Swagger configuration
* Dependency injection setup

---

## Tech Stack

### Backend

* C#
* ASP.NET Core 9
* Entity Framework Core
* PostgreSQL
* FluentValidation
* Serilog
* Swagger / OpenAPI

### Testing

* xUnit
* FluentAssertions

### Planned Frontend

* React
* TypeScript
* Vite
* React Query
* Zustand
* React Router

### Planned DevOps

* Docker
* Docker Compose
* GitHub Actions
* Azure App Service
* Azure Database for PostgreSQL

---

## API Endpoints

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

## Running Tests

```bash
cd src/Backend

dotnet test
```

---

## Project Structure

```text
gartenzwerge-offer-management/
├── docs/
│   ├── api/
│   ├── architecture/
│   └── database/
├── src/
│   └── Backend/
│       ├── Gartenzwerge.Domain/
│       ├── Gartenzwerge.Application/
│       ├── Gartenzwerge.Infrastructure/
│       └── Gartenzwerge.API/
├── tests/
│   └── Gartenzwerge.UnitTests/
├── docker-compose.yml
├── README.md
└── .gitignore
```

## Documentation

Additional project documentation is available in the `docs` folder.

- [API Endpoints](docs/api/endpoints.md)  
  Documents the available REST API endpoints, request bodies, response codes and example payloads.

- [Add Offer Item Flow](docs/business-processes/add-offer-item-flow.md)  
  Explains the business and technical flow for adding an offer item to an existing offer, including price calculation and offer total recalculation.

- [Architecture Documentation](docs/architecture/)  
  Contains documentation about Clean Architecture, request flow and architectural decisions.

- [Database Documentation](docs/database/)  
  Contains documentation about entities, relationships and database-related concepts.

---

## Development Workflow

This project follows a small-step development workflow.

Before committing changes:

- Make sure the project builds successfully
- Run the relevant tests
- Test changed API endpoints through Swagger when applicable
- Keep commits small and focused

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

### v1.0.0 – Full Business Workflow

* Authentication
* Frontend
* Dockerized full-stack setup
* CI/CD
* Deployment
* AI-assisted offer creation

---

## Author

Aaron Decker
