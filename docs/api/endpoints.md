# API Endpoints

This document describes the currently available REST API endpoints of the Gartenzwerge Außenservice backend.

Base URL for local development:

```text
http://localhost:5041
```

Swagger UI is available at:

```text
http://localhost:5041/swagger
```

---

## API Overview

The API supports the core business workflow of the application:

```text
Customer
→ Offer
→ Offer Items
→ Accepted Offer
→ Order
```

The backend is the source of truth for authentication, authorization, validation and business rules.

---

## Authentication

The API uses ASP.NET Core Identity for user management and JWT bearer tokens for authentication.

Authenticated requests must include the JWT token in the `Authorization` header.

```http
Authorization: Bearer <token>
```

Swagger also supports JWT authentication through the `Authorize` button.

---

## Authorization

Most business endpoints require authentication.

The API currently uses two application roles:

| Role     | Purpose                                                                         |
| -------- | ------------------------------------------------------------------------------- |
| Admin    | Full management access including critical delete and service management actions |
| Employee | Regular business workflow access for customers, offers, offer items and orders  |

Authorization behavior:

| Case                              | Response           |
| --------------------------------- | ------------------ |
| Missing or invalid token          | `401 Unauthorized` |
| Valid token but insufficient role | `403 Forbidden`    |

---

## Role Permissions

| Endpoint group                        | Employee | Admin |
| ------------------------------------- | -------- | ----- |
| Customers read/create/update          | Yes      | Yes   |
| Customers delete                      | No       | Yes   |
| Offered Services read                 | Yes      | Yes   |
| Offered Services create/update/delete | No       | Yes   |
| Offers read/create/update             | Yes      | Yes   |
| Offers delete                         | No       | Yes   |
| Offer Items read/create/update/delete | Yes      | Yes   |
| Orders read/create/update             | Yes      | Yes   |
| Orders delete                         | No       | Yes   |

---

## Common Status Codes

| Status code                  | Meaning                                              |
| ---------------------------- | ---------------------------------------------------- |
| `200 OK`                     | Request succeeded                                    |
| `201 Created`                | Resource was created                                 |
| `204 No Content`             | Resource was deleted or no response body is returned |
| `400 Bad Request`            | Validation or business rule error                    |
| `401 Unauthorized`           | Missing or invalid JWT token                         |
| `403 Forbidden`              | Authenticated user has insufficient permissions      |
| `404 Not Found`              | Resource does not exist                              |
| `409 Conflict`               | Request conflicts with existing state                |
| `415 Unsupported Media Type` | Request body or content type is missing or invalid   |
| `500 Internal Server Error`  | Unexpected server error                              |

---

## Validation

Incoming API requests are validated using FluentValidation and application-level business rules.

Typical validation rules include:

* required fields must not be empty
* IDs must not be `00000000-0000-0000-0000-000000000000`
* quantities must be greater than `0`
* prices must not be negative
* validity dates such as `validUntil` must be in the future
* email fields must contain a valid email address if provided
* text fields must not exceed their configured maximum length

Example validation response:

```json
{
  "errors": {
    "FirstName": [
      "'First Name' must not be empty."
    ],
    "Email": [
      "'Email' is not a valid email address."
    ]
  }
}
```

---

# Endpoint Summary

## Authentication Endpoints

| Method | Endpoint             | Auth          | Description                    |
| ------ | -------------------- | ------------- | ------------------------------ |
| `POST` | `/api/auth/register` | Public        | Register a new user            |
| `POST` | `/api/auth/login`    | Public        | Login and receive JWT token    |
| `GET`  | `/api/auth/me`       | Authenticated | Get current authenticated user |

---

## Business Endpoints

| Method   | Endpoint                               | Roles           | Description                      |
| -------- | -------------------------------------- | --------------- | -------------------------------- |
| `GET`    | `/api/customers`                       | Admin, Employee | Get all customers                |
| `GET`    | `/api/customers/{id}`                  | Admin, Employee | Get customer by id               |
| `POST`   | `/api/customers`                       | Admin, Employee | Create customer                  |
| `PUT`    | `/api/customers/{id}`                  | Admin, Employee | Update customer                  |
| `DELETE` | `/api/customers/{id}`                  | Admin           | Soft delete customer             |
| `GET`    | `/api/offered-services`                | Admin, Employee | Get all offered services         |
| `GET`    | `/api/offered-services/{id}`           | Admin, Employee | Get offered service by id        |
| `POST`   | `/api/offered-services`                | Admin           | Create offered service           |
| `PUT`    | `/api/offered-services/{id}`           | Admin           | Update offered service           |
| `DELETE` | `/api/offered-services/{id}`           | Admin           | Soft delete offered service      |
| `GET`    | `/api/offers`                          | Admin, Employee | Get all offers                   |
| `GET`    | `/api/offers/{id}`                     | Admin, Employee | Get offer by id                  |
| `POST`   | `/api/offers`                          | Admin, Employee | Create offer                     |
| `PUT`    | `/api/offers/{id}`                     | Admin, Employee | Update offer                     |
| `DELETE` | `/api/offers/{id}`                     | Admin           | Soft delete offer                |
| `POST`   | `/api/offers/{offerId}/items`          | Admin, Employee | Add offer item                   |
| `GET`    | `/api/offers/{offerId}/items`          | Admin, Employee | Get offer items                  |
| `PUT`    | `/api/offers/{offerId}/items/{itemId}` | Admin, Employee | Update offer item                |
| `DELETE` | `/api/offers/{offerId}/items/{itemId}` | Admin, Employee | Soft delete offer item           |
| `GET`    | `/api/orders`                          | Admin, Employee | Get all orders                   |
| `GET`    | `/api/orders/{id}`                     | Admin, Employee | Get order by id                  |
| `POST`   | `/api/offers/{offerId}/order`          | Admin, Employee | Create order from accepted offer |
| `PUT`    | `/api/orders/{id}`                     | Admin, Employee | Update order                     |
| `DELETE` | `/api/orders/{id}`                     | Admin           | Soft delete order                |

---

# Authentication

## Register User

```http
POST /api/auth/register
```

Creates a new application user and returns an authentication response.

### Request body

```json
{
  "email": "test@gartenzwerge.de",
  "displayName": "Test User",
  "password": "Test1234"
}
```

### Responses

| Status                      | Meaning                             |
| --------------------------- | ----------------------------------- |
| `200 OK`                    | User was registered                 |
| `400 Bad Request`           | Request validation failed           |
| `409 Conflict`              | User with this email already exists |
| `500 Internal Server Error` | Unexpected server error             |

---

## Login User

```http
POST /api/auth/login
```

Authenticates an existing user and returns a JWT token.

### Request body

```json
{
  "email": "test@gartenzwerge.de",
  "password": "Test1234"
}
```

### Responses

| Status                      | Meaning                   |
| --------------------------- | ------------------------- |
| `200 OK`                    | Login successful          |
| `400 Bad Request`           | Request validation failed |
| `401 Unauthorized`          | Invalid email or password |
| `500 Internal Server Error` | Unexpected server error   |

The same error message is returned for unknown emails and wrong passwords to avoid leaking whether an email address exists.

---

## Get Current User

```http
GET /api/auth/me
```

Returns information about the currently authenticated user.

### Example response

```json
{
  "userId": "00000000-0000-0000-0000-000000000000",
  "email": "test@gartenzwerge.de",
  "displayName": "Test User",
  "roles": ["Admin"]
}
```

### Responses

| Status                      | Meaning                  |
| --------------------------- | ------------------------ |
| `200 OK`                    | Current user returned    |
| `401 Unauthorized`          | Missing or invalid token |
| `500 Internal Server Error` | Unexpected server error  |

---

# Customers

Customer endpoints manage customer master data.

## Customer Endpoints

| Method   | Endpoint              | Description                   |
| -------- | --------------------- | ----------------------------- |
| `GET`    | `/api/customers`      | Get all non-deleted customers |
| `GET`    | `/api/customers/{id}` | Get customer by id            |
| `POST`   | `/api/customers`      | Create customer               |
| `PUT`    | `/api/customers/{id}` | Update customer               |
| `DELETE` | `/api/customers/{id}` | Soft delete customer          |

## Create or Update Customer Request

```json
{
  "firstName": "Max",
  "lastName": "Mustermann",
  "company": "Privatkunde",
  "phoneNumber": "0711 123456",
  "email": "max.mustermann@example.com",
  "street": "Gartenstraße",
  "houseNumber": "12",
  "postalCode": "70173",
  "city": "Stuttgart",
  "notes": "Test customer"
}
```

## Response Codes

| Method                       | Success          | Common errors                     |
| ---------------------------- | ---------------- | --------------------------------- |
| `GET /api/customers`         | `200 OK`         | `401`, `403`, `500`               |
| `GET /api/customers/{id}`    | `200 OK`         | `401`, `403`, `404`, `500`        |
| `POST /api/customers`        | `201 Created`    | `400`, `401`, `403`, `500`        |
| `PUT /api/customers/{id}`    | `200 OK`         | `400`, `401`, `403`, `404`, `500` |
| `DELETE /api/customers/{id}` | `204 No Content` | `401`, `403`, `404`, `500`        |

---

# Offered Services

Offered service endpoints manage reusable services that can be used inside offers.

Examples:

* lawn mowing
* hedge cutting
* green waste disposal
* pressure washing

## Offered Service Endpoints

| Method   | Endpoint                     | Description                 |
| -------- | ---------------------------- | --------------------------- |
| `GET`    | `/api/offered-services`      | Get all offered services    |
| `GET`    | `/api/offered-services/{id}` | Get offered service by id   |
| `POST`   | `/api/offered-services`      | Create offered service      |
| `PUT`    | `/api/offered-services/{id}` | Update offered service      |
| `DELETE` | `/api/offered-services/{id}` | Soft delete offered service |

## Create or Update Offered Service Request

```json
{
  "name": "Rasen mähen",
  "description": "Mowing lawn areas based on square meters.",
  "unit": "m²",
  "basePrice": 0.18,
  "isActive": true
}
```

## Response Codes

| Method                              | Success          | Common errors                     |
| ----------------------------------- | ---------------- | --------------------------------- |
| `GET /api/offered-services`         | `200 OK`         | `401`, `403`, `500`               |
| `GET /api/offered-services/{id}`    | `200 OK`         | `401`, `403`, `404`, `500`        |
| `POST /api/offered-services`        | `201 Created`    | `400`, `401`, `403`, `500`        |
| `PUT /api/offered-services/{id}`    | `200 OK`         | `400`, `401`, `403`, `404`, `500` |
| `DELETE /api/offered-services/{id}` | `204 No Content` | `401`, `403`, `404`, `500`        |

---

# Offers

Offer endpoints manage customer offers.

An offer belongs to one customer and can contain multiple offer items.

## Offer Status Values

| Value | Status     | Meaning                            |
| ----- | ---------- | ---------------------------------- |
| `1`   | `Draft`    | Offer is being prepared            |
| `2`   | `Sent`     | Offer was sent to the customer     |
| `3`   | `Accepted` | Offer was accepted by the customer |
| `4`   | `Rejected` | Offer was rejected                 |

## Offer Endpoints

| Method   | Endpoint           | Description                |
| -------- | ------------------ | -------------------------- |
| `GET`    | `/api/offers`      | Get all non-deleted offers |
| `GET`    | `/api/offers/{id}` | Get offer by id            |
| `POST`   | `/api/offers`      | Create offer               |
| `PUT`    | `/api/offers/{id}` | Update offer               |
| `DELETE` | `/api/offers/{id}` | Soft delete offer          |

## Create Offer Request

```json
{
  "customerId": "00000000-0000-0000-0000-000000000000",
  "validUntil": "2026-12-31T00:00:00Z",
  "notes": "Test offer for garden maintenance."
}
```

## Update Offer Request

```json
{
  "validUntil": "2026-12-31T00:00:00Z",
  "status": 3,
  "notes": "Customer accepted the offer."
}
```

## Offer Response Example

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "offerNumber": "O-20260626-123456",
  "customerId": "00000000-0000-0000-0000-000000000000",
  "customerName": "Max Mustermann",
  "createdAt": "2026-06-26T10:00:00Z",
  "validUntil": "2026-12-31T00:00:00Z",
  "status": 3,
  "totalNet": 305.00,
  "notes": "Customer accepted the offer."
}
```

## Server-Side Behavior

When an offer is created:

* the backend verifies that the customer exists
* an offer number is generated
* the initial status is set to `Draft`
* the initial total value is set to `0`

## Response Codes

| Method                    | Success          | Common errors                     |
| ------------------------- | ---------------- | --------------------------------- |
| `GET /api/offers`         | `200 OK`         | `401`, `403`, `500`               |
| `GET /api/offers/{id}`    | `200 OK`         | `401`, `403`, `404`, `500`        |
| `POST /api/offers`        | `201 Created`    | `400`, `401`, `403`, `404`, `500` |
| `PUT /api/offers/{id}`    | `200 OK`         | `400`, `401`, `403`, `404`, `500` |
| `DELETE /api/offers/{id}` | `204 No Content` | `401`, `403`, `404`, `500`        |

---

# Offer Items

Offer item endpoints manage service positions inside offers.

An offer item belongs to exactly one offer and references one offered service.

## Offer Item Endpoints

| Method   | Endpoint                               | Description            |
| -------- | -------------------------------------- | ---------------------- |
| `POST`   | `/api/offers/{offerId}/items`          | Add item to offer      |
| `GET`    | `/api/offers/{offerId}/items`          | Get offer items        |
| `PUT`    | `/api/offers/{offerId}/items/{itemId}` | Update item quantity   |
| `DELETE` | `/api/offers/{offerId}/items/{itemId}` | Soft delete offer item |

## Create Offer Item Request

```json
{
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "quantity": 250
}
```

## Update Offer Item Request

```json
{
  "quantity": 300
}
```

## Offer Item Response Example

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "offerId": "00000000-0000-0000-0000-000000000000",
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "description": "Rasen mähen",
  "quantity": 250,
  "unit": "m²",
  "unitPrice": 0.18,
  "totalPrice": 45.00
}
```

## Calculation Behavior

When an offer item is created or updated:

```text
Item total = Quantity × Unit price
Offer total = Sum of active offer item totals
```

The backend uses the offered service base price as the item unit price and recalculates the offer total after item changes.

## Response Codes

| Method                                        | Success          | Common errors                     |
| --------------------------------------------- | ---------------- | --------------------------------- |
| `POST /api/offers/{offerId}/items`            | `201 Created`    | `400`, `401`, `403`, `404`, `500` |
| `GET /api/offers/{offerId}/items`             | `200 OK`         | `401`, `403`, `404`, `500`        |
| `PUT /api/offers/{offerId}/items/{itemId}`    | `200 OK`         | `400`, `401`, `403`, `404`, `500` |
| `DELETE /api/offers/{offerId}/items/{itemId}` | `204 No Content` | `401`, `403`, `404`, `500`        |

---

# Orders

Order endpoints manage service orders.

An order is created from an accepted offer. Each offer can only be converted into one order.

## Order Status Values

| Value | Status       | Meaning                                 |
| ----- | ------------ | --------------------------------------- |
| `1`   | `Planned`    | Order exists but is not yet in progress |
| `2`   | `InProgress` | Order is currently being worked on      |
| `3`   | `Completed`  | Order has been completed                |
| `4`   | `Cancelled`  | Order was cancelled                     |

## Order Endpoints

| Method   | Endpoint                      | Description                      |
| -------- | ----------------------------- | -------------------------------- |
| `GET`    | `/api/orders`                 | Get all non-deleted orders       |
| `GET`    | `/api/orders/{id}`            | Get order by id                  |
| `POST`   | `/api/offers/{offerId}/order` | Create order from accepted offer |
| `PUT`    | `/api/orders/{id}`            | Update order                     |
| `DELETE` | `/api/orders/{id}`            | Soft delete order                |

## Create Order From Offer Request

```http
POST /api/offers/{offerId}/order
```

Current frontend request body:

```json
{}
```

The endpoint expects a JSON request body. The current frontend sends an empty object because order creation is based on the accepted offer.

If the backend request model later exposes optional order fields, the request may also include values such as `plannedDate` or `notes`.

## Create Order From Offer Behavior

The backend:

* loads the offer by id
* verifies that the offer exists
* verifies that the offer status is `Accepted`
* checks that no order already exists for the offer
* creates a new order with status `Planned`
* copies the `OfferId` and `CustomerId` from the offer
* returns the created order

## Order Response Example

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "offerId": "00000000-0000-0000-0000-000000000000",
  "customerId": "00000000-0000-0000-0000-000000000000",
  "status": 1,
  "plannedDate": null,
  "completedAt": null,
  "notes": null
}
```

## Update Order Request

```json
{
  "status": 2,
  "plannedDate": "2026-08-05T09:00:00Z",
  "notes": "Order is now in progress."
}
```

## Update Order Behavior

When an order is updated, the backend:

* updates the order status
* updates the planned date
* updates the notes
* sets `completedAt` automatically when the status is `Completed`
* clears `completedAt` when the status is changed away from `Completed`

## Response Codes

| Method                             | Success          | Common errors                                   |
| ---------------------------------- | ---------------- | ----------------------------------------------- |
| `GET /api/orders`                  | `200 OK`         | `401`, `403`, `500`                             |
| `GET /api/orders/{id}`             | `200 OK`         | `401`, `403`, `404`, `500`                      |
| `POST /api/offers/{offerId}/order` | `201 Created`    | `400`, `401`, `403`, `404`, `409`, `415`, `500` |
| `PUT /api/orders/{id}`             | `200 OK`         | `400`, `401`, `403`, `404`, `500`               |
| `DELETE /api/orders/{id}`          | `204 No Content` | `401`, `403`, `404`, `500`                      |

## Order Creation Error Cases

| Case                              | Expected behavior                                  |
| --------------------------------- | -------------------------------------------------- |
| Offer does not exist              | Request fails                                      |
| Offer is not accepted             | Request fails                                      |
| Order already exists for offer    | Request fails with conflict or business error      |
| Missing JSON body or content type | Request may fail with `415 Unsupported Media Type` |
| Missing authentication            | Request fails with `401 Unauthorized`              |
| Missing role permission           | Request fails with `403 Forbidden`                 |

---

# Current Limitations

The current API supports authentication, authorization, customer management, offered service management, offer management, offer item management and core order management.

Known limitations:

* no refresh tokens
* no password reset flow
* no email confirmation
* no advanced pricing rules
* no tiered pricing
* no PDF generation for offers
* no dedicated complete or cancel order endpoints yet
* no employee assignment for orders yet

---

# Related Documentation

* [Authentication Architecture](../architecture/authentication.md)
* [Request Flow](../architecture/request-flow.md)
* [Offer-to-Order Workflow](../business-processes/offer-to-order-workflow.md)
* [Create Order From Offer Flow](../business-processes/create-order-from-offer-flow.md)
* [Add Offer Item Flow](../business-processes/add-offer-item-flow.md)
* [Frontend Architecture](../frontend/frontend-architecture.md)
