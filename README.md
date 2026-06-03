# Gartenzwerge Außenservice – Angebots- und Auftragsmanagement

A modern full-stack business management application for managing customers, offers and service orders for a landscaping and outdoor service business.

The application is designed to support real business workflows such as customer management, offer creation, service pricing and order handling while demonstrating clean architecture and professional software engineering practices.

---

## Current Status

🚧 Active Development

### Completed

* Customer CRUD API
* PostgreSQL integration
* Entity Framework Core persistence
* Repository Pattern
* Service Layer
* FluentValidation request validation
* Global exception handling
* Serilog logging
* Unit tests
* Swagger/OpenAPI documentation
* Offered Service CRUD API
* Offered Service validation
* Offered Service unit tests

### Planned

* Service Management
* Offer Management
* Offer Item Management
* Pricing Calculator
* Order Management
* Authentication & Authorization
* Docker & Docker Compose
* GitHub Actions CI/CD
* Azure Deployment
* OpenAI Integration

---

## Features

### Customer Management

* Create customers
* View customers
* Update customers
* Soft delete customers

### Validation

* Request validation using FluentValidation
* Required field validation
* Email format validation
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
* Service
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
* Add offer items
* Calculate offer totals
* Manage offer status

### v0.4.0 – Pricing Calculator

* Lawn mowing price calculation
* Hedge cutting price calculation
* Green waste disposal calculation
* Travel cost calculation

### v0.5.0 – Order Management

* Convert accepted offers into orders
* Manage order status
* Complete or cancel orders

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
