# Request Flow

This document explains the typical request flow through the Gartenzwerge offer management backend.

---

## Overview

The application follows a layered architecture.

A typical API request flows through the following layers:

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
Entity Framework Core / PostgreSQL
 ↓
HTTP Response
```

---

## Example: Create Order From Offer

Endpoint:

```http
POST /api/offers/{offerId}/order
```

This request creates a new order from an accepted offer.

---

## Step 1: HTTP Request

The client sends a request to the API.

Example:

```json
{
  "plannedDate": "2026-08-01T09:00:00Z",
  "notes": "First order created from accepted offer."
}
```

---

## Step 2: Controller

The controller receives the request and forwards it to the application service.

Example responsibility:

* Read route parameters
* Read request body
* Call the correct application service method
* Return the correct HTTP response

Controllers should stay thin and should not contain business logic.

---

## Step 3: FluentValidation

Request DTOs are validated automatically using FluentValidation.

Examples:

* Required fields must not be empty
* IDs must not be empty GUIDs
* Quantities must be greater than zero
* Dates must be in the future when required
* Text fields must not exceed maximum lengths
* Email fields must be valid when provided

Invalid requests return:

```http
400 Bad Request
```

This happens before the application service logic is executed.

---

## Step 4: Application Service

The application service contains the use case and business rules.

For creating an order from an offer, the service verifies:

* The offer exists
* The offer status is `Accepted`
* No order already exists for the offer

If all rules pass, a new order is created with:

* `OfferId` from the offer
* `CustomerId` from the offer
* `Status = Planned`
* Optional planned date
* Optional notes

---

## Step 5: Repository Interface

The application service depends on repository interfaces.

Example:

```text
IOrderRepository
IOfferRepository
```

This keeps the Application layer independent from Entity Framework Core and PostgreSQL.

The Application layer defines what it needs.

The Infrastructure layer defines how it is implemented.

---

## Step 6: Repository Implementation

The repository implementation uses Entity Framework Core and the `AppDbContext` to access the database.

Example responsibilities:

* Add entities
* Load entities by ID
* Load related data when needed
* Save changes
* Apply soft-delete filters

---

## Step 7: Database

PostgreSQL stores the application data.

Entity Framework Core maps domain entities to database tables.

Soft-deleted records are not physically removed. Instead, they are marked with:

```text
IsDeleted = true
DeletedAt = timestamp
```

---

## Step 8: Response

If the request succeeds, the controller returns a success response.

Examples:

```http
200 OK
201 Created
204 No Content
```

If a business error occurs, the global exception middleware maps application exceptions to HTTP responses.

---

## Exception Handling

The API uses global exception middleware.

Known application exceptions are mapped to meaningful HTTP responses.

```text
NotFoundException  → 404 Not Found
ConflictException  → 409 Conflict
Validation errors  → 400 Bad Request
Unexpected errors  → 500 Internal Server Error
```

Example conflict cases:

* Creating an order from a non-accepted offer
* Creating a duplicate order for the same offer

---

## Responsibilities by Layer

### API Layer

* Controllers
* Middleware
* Swagger setup
* Dependency injection setup

### Application Layer

* DTOs
* Validators
* Service interfaces
* Application services
* Repository interfaces
* Business rules

### Domain Layer

* Entities
* Enums
* Core business concepts
* Shared base entity fields

### Infrastructure Layer

* Entity Framework Core
* PostgreSQL persistence
* Repository implementations
* Database configuration

---

## Key Principle

Business rules should live in the Application layer, not in controllers.

Controllers should delegate work to services.

Repositories should handle persistence, not business decisions.
