# Request Flow

This document explains how requests flow through the Gartenzwerge management application.

It focuses on the interaction between frontend, API, application services, repositories and the database.

---

## Goal

The goal of this document is to make the runtime behavior of the application understandable.

It shows how a request moves through the system and which layer is responsible for which part of the work.

---

## General Backend Request Flow

A typical backend API request follows this structure:

```text
HTTP Request
→ Controller
→ FluentValidation
→ Application Service
→ Repository Interface
→ Repository Implementation
→ Entity Framework Core
→ PostgreSQL
→ HTTP Response
```

Visualized as a sequence:

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Validator
    participant Service
    participant Repository
    participant DbContext
    participant Database

    Client->>Controller: HTTP request
    Controller->>Validator: Validate request DTO
    Validator-->>Controller: Validation result
    Controller->>Service: Call application use case
    Service->>Repository: Use repository interface
    Repository->>DbContext: Query or persist data
    DbContext->>Database: Execute SQL
    Database-->>DbContext: Result
    DbContext-->>Repository: Entity data
    Repository-->>Service: Domain or DTO result
    Service-->>Controller: Application result
    Controller-->>Client: HTTP response
```

---

## Layer Responsibilities

| Layer          | Responsibility                                                  |
| -------------- | --------------------------------------------------------------- |
| Frontend       | Calls API endpoints and displays user-facing state              |
| API            | Receives HTTP requests and returns HTTP responses               |
| Application    | Executes use cases and business rules                           |
| Domain         | Contains core entities and enums                                |
| Infrastructure | Implements persistence, Identity and external technical details |
| Database       | Stores application data                                         |

---

## Authentication Request Flow

Authentication requests are public for registration and login, but still pass through validation and application services.

### Login Flow

```mermaid
sequenceDiagram
    participant Frontend
    participant AuthController
    participant Validator
    participant AuthService
    participant Identity
    participant JWT

    Frontend->>AuthController: POST /api/auth/login
    AuthController->>Validator: Validate LoginRequest
    Validator-->>AuthController: Valid request
    AuthController->>AuthService: LoginAsync
    AuthService->>Identity: Find user by email
    AuthService->>Identity: Check password
    Identity-->>AuthService: Password valid
    AuthService->>JWT: Generate signed JWT token
    JWT-->>AuthService: Token
    AuthService-->>AuthController: AuthResponse
    AuthController-->>Frontend: 200 OK with token
```

After a successful login, the frontend stores the JWT token and redirects the user to the protected app area.

```text
Login success
→ token stored in localStorage
→ redirect to /dashboard
→ load current user through /api/auth/me
```

---

## Authenticated Request Flow

Protected requests require a valid JWT bearer token.

```http
Authorization: Bearer <token>
```

```mermaid
sequenceDiagram
    participant Frontend
    participant JwtMiddleware
    participant Authorization
    participant Controller
    participant Service

    Frontend->>JwtMiddleware: Request with Bearer token
    JwtMiddleware->>JwtMiddleware: Validate token signature and lifetime
    JwtMiddleware->>JwtMiddleware: Build authenticated user principal
    JwtMiddleware->>Authorization: Check [Authorize] requirements
    Authorization-->>Controller: Access granted
    Controller->>Service: Execute use case
    Service-->>Controller: Result
    Controller-->>Frontend: HTTP response
```

If the token is missing or invalid, the request does not reach the controller action.

---

## Role-Based Request Flow

Some endpoints require a specific role.

Example:

```csharp
[Authorize(Roles = ApplicationRoles.Admin)]
```

```mermaid
sequenceDiagram
    participant Frontend
    participant JwtMiddleware
    participant Authorization
    participant Controller

    Frontend->>JwtMiddleware: Request with Bearer token
    JwtMiddleware->>JwtMiddleware: Validate token
    JwtMiddleware->>Authorization: Read role claims
    Authorization->>Authorization: Check required role
    alt User has required role
        Authorization-->>Controller: Access granted
        Controller-->>Frontend: Success response
    else User does not have required role
        Authorization-->>Frontend: 403 Forbidden
    end
```

Authorization behavior:

| Situation                             | Response                      |
| ------------------------------------- | ----------------------------- |
| Missing token                         | `401 Unauthorized`            |
| Invalid token                         | `401 Unauthorized`            |
| Valid token but missing required role | `403 Forbidden`               |
| Valid token with required role        | Controller action is executed |

---

# Business Flow Examples

## Example 1: Add Offer Item

Endpoint:

```http
POST /api/offers/{offerId}/items
```

This request adds a service position to an existing offer.

### Request Body

```json
{
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "quantity": 250
}
```

### Flow

```mermaid
sequenceDiagram
    participant Frontend
    participant OfferItemsController
    participant Validator
    participant OfferItemService
    participant OfferRepository
    participant OfferedServiceRepository
    participant Database

    Frontend->>OfferItemsController: POST /api/offers/{offerId}/items
    OfferItemsController->>Validator: Validate request
    Validator-->>OfferItemsController: Valid request
    OfferItemsController->>OfferItemService: Add item to offer
    OfferItemService->>OfferRepository: Load offer with items
    OfferRepository->>Database: Query offer
    Database-->>OfferRepository: Offer
    OfferItemService->>OfferedServiceRepository: Load offered service
    OfferedServiceRepository->>Database: Query offered service
    Database-->>OfferedServiceRepository: Offered service
    OfferItemService->>OfferItemService: Calculate item total
    OfferItemService->>OfferItemService: Recalculate offer total
    OfferItemService->>OfferRepository: Save updated offer
    OfferRepository->>Database: Persist changes
    OfferItemService-->>OfferItemsController: Offer item DTO
    OfferItemsController-->>Frontend: 201 Created
```

### Business Rules

| Rule                                           | Purpose                                      |
| ---------------------------------------------- | -------------------------------------------- |
| Offer must exist                               | Items can only be added to existing offers   |
| Offered service must exist                     | Item pricing depends on the selected service |
| Quantity must be greater than zero             | Prevents invalid totals                      |
| Item total is calculated by the backend        | Backend remains source of truth              |
| Offer total is recalculated after item changes | Keeps offer totals consistent                |

---

## Example 2: Create Order From Offer

Endpoint:

```http
POST /api/offers/{offerId}/order
```

This request creates a new order from an accepted offer.

The current frontend sends an empty JSON object because order creation is based on the accepted offer.

```json
{}
```

### Flow

```mermaid
sequenceDiagram
    participant Frontend
    participant OrdersController
    participant Validator
    participant OrderService
    participant OfferRepository
    participant OrderRepository
    participant Database

    Frontend->>OrdersController: POST /api/offers/{offerId}/order
    OrdersController->>Validator: Validate request
    Validator-->>OrdersController: Valid request
    OrdersController->>OrderService: Create order from offer
    OrderService->>OfferRepository: Load offer by id
    OfferRepository->>Database: Query offer
    Database-->>OfferRepository: Offer
    OrderService->>OrderService: Check offer status
    OrderService->>OrderRepository: Check existing order for offer
    OrderRepository->>Database: Query order by offer id
    Database-->>OrderRepository: No existing order
    OrderService->>OrderService: Create order with status Planned
    OrderService->>OrderRepository: Save order
    OrderRepository->>Database: Insert order
    OrderService-->>OrdersController: Order DTO
    OrdersController-->>Frontend: 201 Created
```

### Business Rules

| Rule                                                 | Purpose                                                      |
| ---------------------------------------------------- | ------------------------------------------------------------ |
| Offer must exist                                     | Orders can only be created from real offers                  |
| Offer must be accepted                               | Prevents draft, sent or rejected offers from becoming orders |
| Only one order per offer is allowed                  | Prevents duplicate operational work                          |
| New orders start as `Planned`                        | Establishes a clear order lifecycle                          |
| `OfferId` and `CustomerId` are copied from the offer | Maintains business traceability                              |

---

## Example 3: Offer Acceptance and Order Conversion from Frontend

The frontend intentionally uses an explicit conversion action instead of silently creating an order when the status changes.

User action:

```text
Angebot annehmen & Auftrag erstellen
```

### Flow

```mermaid
sequenceDiagram
    participant User
    participant OfferDetailsPage
    participant OffersAPI
    participant OrdersAPI
    participant Backend

    User->>OfferDetailsPage: Click conversion button
    OfferDetailsPage->>OffersAPI: PUT /api/offers/{offerId}
    OffersAPI->>Backend: Update offer status to Accepted
    Backend-->>OffersAPI: Updated offer
    OffersAPI-->>OfferDetailsPage: Success
    OfferDetailsPage->>OrdersAPI: POST /api/offers/{offerId}/order
    OrdersAPI->>Backend: Create order from accepted offer
    Backend-->>OrdersAPI: Created order
    OrdersAPI-->>OfferDetailsPage: Success
    OfferDetailsPage-->>User: Redirect to /orders
```

If an order already exists for the offer, the frontend does not show the creation button and links to the existing order instead.

---

## Example 4: Load Orders Overview

Route:

```text
/orders
```

The current `OrderDto` is intentionally lightweight. It does not contain all display data needed by the frontend.

Therefore, the frontend loads orders and offers, then combines them for display.

### Flow

```mermaid
sequenceDiagram
    participant OrdersPage
    participant OrdersAPI
    participant OffersAPI
    participant Backend
    participant UI

    OrdersPage->>OrdersAPI: GET /api/orders
    OrdersAPI->>Backend: Load orders
    Backend-->>OrdersAPI: Order list
    OrdersAPI-->>OrdersPage: Orders

    OrdersPage->>OffersAPI: GET /api/offers
    OffersAPI->>Backend: Load offers
    Backend-->>OffersAPI: Offer list
    OffersAPI-->>OrdersPage: Offers

    OrdersPage->>OrdersPage: Match order.offerId with offer.id
    OrdersPage->>UI: Display enriched order cards
```

### Display Data Sources

| Displayed information | Source        |
| --------------------- | ------------- |
| Order status          | Order         |
| Planned date          | Order         |
| Completed date        | Order         |
| Customer name         | Related offer |
| Offer number          | Related offer |
| Total amount          | Related offer |

This keeps the order API simple while still making the frontend useful.

---

## Example 5: Load Order Details

Route:

```text
/orders/:orderId
```

The order details page is currently read-only.

### Flow

```mermaid
sequenceDiagram
    participant OrderDetailsPage
    participant OrdersAPI
    participant OffersAPI
    participant OfferItemsAPI
    participant Backend

    OrderDetailsPage->>OrdersAPI: GET /api/orders/{orderId}
    OrdersAPI->>Backend: Load order by id
    Backend-->>OrdersAPI: Order
    OrdersAPI-->>OrderDetailsPage: Order

    OrderDetailsPage->>OffersAPI: GET /api/offers/{offerId}
    OffersAPI->>Backend: Load related offer
    Backend-->>OffersAPI: Offer
    OffersAPI-->>OrderDetailsPage: Offer

    OrderDetailsPage->>OfferItemsAPI: GET /api/offers/{offerId}/items
    OfferItemsAPI->>Backend: Load offer items
    Backend-->>OfferItemsAPI: Offer items
    OfferItemsAPI-->>OrderDetailsPage: Offer items
```

The page displays:

* order data
* related offer foundation
* original offer items
* links back to the order overview and related offer

---

# Error Handling Flow

The API uses validation, application exceptions and ASP.NET Core middleware to return meaningful HTTP responses.

```mermaid
flowchart TD
    A[Request received] --> B{Valid request body?}
    B -->|No| C[400 Bad Request]
    B -->|Yes| D{Authenticated if required?}
    D -->|No| E[401 Unauthorized]
    D -->|Yes| F{Required role?}
    F -->|Missing role| G[403 Forbidden]
    F -->|Allowed| H[Execute application service]
    H --> I{Business rule passes?}
    I -->|No| J[400 or 409 Business error]
    I -->|Yes| K[Return success response]
    H --> L{Resource found?}
    L -->|No| M[404 Not Found]
    L -->|Yes| K
```

---

## Common Error Responses

| Error source              | Example                      | Response                            |
| ------------------------- | ---------------------------- | ----------------------------------- |
| Validation                | Empty required field         | `400 Bad Request`                   |
| Authentication middleware | Missing or invalid token     | `401 Unauthorized`                  |
| Authorization middleware  | Valid token but missing role | `403 Forbidden`                     |
| Application service       | Resource does not exist      | `404 Not Found`                     |
| Application service       | Duplicate order for offer    | `409 Conflict`                      |
| Application service       | Business rule violation      | `400 Bad Request` or `409 Conflict` |
| Unexpected error          | Unhandled exception          | `500 Internal Server Error`         |

---

# Success Responses

| Response         | Typical use                                 |
| ---------------- | ------------------------------------------- |
| `200 OK`         | Read or update succeeded                    |
| `201 Created`    | Resource was created                        |
| `204 No Content` | Resource was deleted or no body is returned |

---

# Key Principles

| Principle                          | Meaning                                                  |
| ---------------------------------- | -------------------------------------------------------- |
| Controllers stay thin              | Controllers delegate work to application services        |
| Business rules live in Application | Core decisions are not hidden in controllers             |
| Repositories handle persistence    | Database logic is separated from use cases               |
| Backend is source of truth         | Frontend improves UX but does not enforce security alone |
| Validation protects use cases      | Invalid input is rejected before business logic runs     |
| Errors are mapped consistently     | Expected failures become meaningful HTTP responses       |

---

# Related Documentation

* [Clean Architecture](clean-architecture.md)
* [Authentication Architecture](authentication.md)
* [API Endpoints](../api/endpoints.md)
* [Offer-to-Order Workflow](../business-processes/offer-to-order-workflow.md)
* [Create Order From Offer Flow](../business-processes/create-order-from-offer-flow.md)
* [Add Offer Item Flow](../business-processes/add-offer-item-flow.md)
