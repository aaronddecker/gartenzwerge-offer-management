# Clean Architecture

This document explains how Clean Architecture is applied in the Gartenzwerge offer management backend.

---

## Goal

The goal of the architecture is to keep business logic independent from technical details.

The application should not depend directly on databases, frameworks or external systems.

Instead, the core logic is placed in the Domain and Application layers, while infrastructure details are kept outside.

---

## Project Structure

The backend is separated into the following projects:

```text
Gartenzwerge.Domain
Gartenzwerge.Application
Gartenzwerge.Infrastructure
Gartenzwerge.API
```

---

## Dependency Direction

The dependency direction points inward.

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

The `Domain` layer does not depend on any other layer.

The `Application` layer depends on the `Domain` layer.

The `Infrastructure` layer depends on `Application` and `Domain`.

The `API` layer depends on `Application`, `Infrastructure` and `Domain`.

---

## Domain Layer

Project:

```text
Gartenzwerge.Domain
```

The Domain layer contains the core business concepts.

Examples:

* `Customer`
* `OfferedService`
* `Offer`
* `OfferItem`
* `Order`
* `OfferStatus`
* `OrderStatus`
* `BaseEntity`

The Domain layer should not contain database logic, API logic or framework-specific code.

---

## Application Layer

Project:

```text
Gartenzwerge.Application
```

The Application layer contains use cases and business rules.

Examples:

* Creating customers
* Creating offered services
* Creating offers
* Adding offer items
* Updating offer items
* Soft deleting offer items
* Creating orders from accepted offers
* Updating orders
* Soft deleting orders
* Registering users
* Logging in users
* Validating authentication requests


The Application layer contains:

* DTOs
* Validators
* Service interfaces
* Service implementations
* Repository interfaces
* Application exceptions

Examples:

```text
ICustomerService
IOfferService
IOfferItemService
IOrderService

ICustomerRepository
IOfferRepository
IOfferedServiceRepository
IOrderRepository

NotFoundException
ConflictException

IAuthService

RegisterRequest
LoginRequest
AuthResponse

RegisterRequestValidator
LoginRequestValidator

UnauthorizedException
```

Business rules belong here.

Examples:

* An order can only be created from an accepted offer.
* Each offer can only have one order.
* Offer totals are recalculated from active offer items.
* `completedAt` is set when an order is completed.
* `completedAt` is cleared when an order is reopened.
* Duplicate user registration is prevented by checking existing email addresses.
* Invalid login attempts return the same error message for unknown emails and wrong passwords.


---

## Infrastructure Layer

Project:

```text
Gartenzwerge.Infrastructure
```

The Infrastructure layer contains technical implementations.

Examples:

* Entity Framework Core
* PostgreSQL access
* Repository implementations
* `AppDbContext`
* Database configuration
* Dependency injection for persistence
* ASP.NET Core Identity
* `ApplicationUser`
* Identity database schema
* JWT token generation
* `AuthService`


Authentication-related technical details also live in the Infrastructure layer.

Example:

```text
Application:
IAuthService

Infrastructure:
AuthService
ASP.NET Core Identity
JWT token generation
```

This keeps the Application layer independent from ASP.NET Core Identity and JWT implementation details.


---

## API Layer

Project:

```text
Gartenzwerge.API
```

The API layer exposes the application through HTTP endpoints.

Examples:

* `CustomersController`
* `OfferedServicesController`
* `OffersController`
* `OfferItemsController`
* `OrdersController`
* `AuthController`


The API layer is responsible for:

* Receiving HTTP requests
* Reading route parameters
* Reading request bodies
* Calling application services
* Returning HTTP responses
* Configuring Swagger
* Configuring middleware
* Registering dependencies
* Configuring JWT bearer authentication
* Configuring Swagger JWT authorization

Controllers should stay thin.

They should not contain business rules.

---

## Request Flow

A typical request flows through the application like this:

```text
HTTP Request
 ↓
Controller
 ↓
FluentValidation
 ↓
Application Service
 ↓
Repository Interface
 ↓
Repository Implementation
 ↓
AppDbContext
 ↓
PostgreSQL
```

Example:

```text
POST /api/offers/{offerId}/order
 ↓
OrdersController
 ↓
CreateOrderFromOfferRequestValidator
 ↓
OrderService
 ↓
IOfferRepository / IOrderRepository
 ↓
OfferRepository / OrderRepository
 ↓
AppDbContext
 ↓
PostgreSQL
```

Authentication example:

```text
POST /api/auth/login
 ↓
AuthController
 ↓
LoginRequestValidator
 ↓
IAuthService
 ↓
AuthService
 ↓
UserManager<ApplicationUser>
 ↓
ASP.NET Core Identity
 ↓
JWT token response
```

Authenticated request example:

```text
GET /api/auth/me
 ↓
JWT Bearer Authentication Middleware
 ↓
[Authorize]
 ↓
AuthController
 ↓
Current user information
```

---

## Repository Pattern

Repositories are used to separate business logic from database access.

The Application layer only knows repository interfaces.

The Infrastructure layer provides the actual implementations.

This makes the Application layer independent from Entity Framework Core.

Example:

```text
OrderService depends on IOrderRepository.
OrderService does not depend on AppDbContext.
```

---

## Validation

Request validation is handled with FluentValidation.

Validators live in the Application layer because they protect application use cases from invalid input.

Examples:

* Customer request validation
* Offered service request validation
* Offer request validation
* Offer item request validation
* Order request validation
* Register request validation
* Login request validation

Invalid request bodies return:

```http
400 Bad Request
```

---

## Exception Handling

The application uses custom exceptions for expected business errors.

Examples:

```text
NotFoundException
ConflictException
UnauthorizedException
```

The API layer maps these exceptions to HTTP responses through global exception middleware.

```text
NotFoundException      → 404 Not Found
UnauthorizedException  → 401 Unauthorized
ConflictException      → 409 Conflict
Unexpected errors      → 500 Internal Server Error
```


This keeps controllers clean because they do not need to handle every error case manually.

---

## Soft Delete

Some entities are soft-deleted instead of physically removed from the database.

Soft delete means:

```text
IsDeleted = true
DeletedAt = timestamp
```

This keeps historical data in the database while hiding deleted records from normal queries.

Currently used for:

* Offer items
* Orders

---

## Benefits

This architecture provides several benefits:

* Business logic is separated from technical details.
* Controllers stay small and focused.
* Database access is isolated in the Infrastructure layer.
* The Application layer can be tested without a real database.
* Business rules are easier to find and maintain.
* New features can follow the same structure consistently.

---

## Rule of Thumb

Use this rule when deciding where code belongs:

```text
Entity or enum?                         → Domain
Use case, DTO, validator or interface?  → Application
Database, Identity or JWT detail?       → Infrastructure
HTTP endpoint, middleware or Swagger?   → API
```
