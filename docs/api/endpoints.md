# API Endpoints

This document describes the currently available REST API endpoints of the Gartenzwerge Auﬂenservice offer management backend.

Base URL for local development:

```text
http://localhost:5041
```

Swagger UI is available at:

```text
http://localhost:5041/swagger
```

---

## Validation

Incoming API requests are validated automatically using FluentValidation.

If a request contains invalid input, the API returns:

```http
400 Bad Request
```

Typical validation rules include:

* Required fields must not be empty
* IDs must not be `00000000-0000-0000-0000-000000000000`
* Quantities must be greater than `0`
* Prices must not be negative
* Validity dates such as `validUntil` must be in the future
* Email fields must contain a valid email address if provided
* Text fields must not exceed their configured maximum length

Example invalid request:

```json
{
  "firstName": "",
  "lastName": "Mustermann",
  "email": "not-an-email"
}
```

Example response:

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

## Authentication

Authentication endpoints are used to register users, authenticate existing users and retrieve information about the currently authenticated user.

The API uses ASP.NET Core Identity for user management and JWT bearer tokens for authentication.

Authenticated requests must include the JWT token in the `Authorization` header.

```http
Authorization: Bearer <token>
```

Swagger also supports JWT authentication through the `Authorize` button.

---

### Register user

```http
POST /api/auth/register
```

Creates a new application user and returns a JWT token.

#### Request body

```json
{
  "email": "test@gartenzwerge.de",
  "displayName": "Test User",
  "password": "Test1234"
}
```

#### Server-side behavior

* Validates the request body
* Checks whether a user with the same email already exists
* Creates a new Identity user
* Hashes the password using ASP.NET Core Identity
* Returns an authentication response with a JWT token

#### Responses

```http
200 OK
400 Bad Request
409 Conflict
500 Internal Server Error
```

#### Conflict cases

```text
A user with this email already exists.
```

---

### Login user

```http
POST /api/auth/login
```

Authenticates an existing user and returns a JWT token.

#### Request body

```json
{
  "email": "test@gartenzwerge.de",
  "password": "Test1234"
}
```

#### Server-side behavior

* Validates the request body
* Looks up the user by email
* Checks the password using ASP.NET Core Identity
* Returns an authentication response with a JWT token

#### Responses

```http
200 OK
400 Bad Request
401 Unauthorized
500 Internal Server Error
```

#### Unauthorized cases

```text
Invalid email or password.
```

The same error message is returned for unknown emails and wrong passwords to avoid leaking whether an email address exists.

---

### Get current authenticated user

```http
GET /api/auth/me
```

Returns information about the currently authenticated user.

This endpoint requires a valid JWT bearer token.

#### Authorization header

```http
Authorization: Bearer <token>
```

#### Responses

```http
200 OK
401 Unauthorized
500 Internal Server Error
```

#### Example response

```json
{
  "userId": "00000000-0000-0000-0000-000000000000",
  "email": "test@gartenzwerge.de",
  "displayName": "Test User"
}
```
---

## Protected Business Endpoints

Most business endpoints require authentication.

Authenticated requests must include a valid JWT bearer token.

```http
Authorization: Bearer <token>
```

Protected endpoint groups:

* Customers
* Offered Services
* Offers
* Offer Items
* Orders

Public endpoint groups:

* Register user
* Login user

The `/api/auth/me` endpoint is also protected and can be used to verify that a JWT token is valid.

If a protected endpoint is called without a valid token, the API returns:

```http
401 Unauthorized
```

## Customers

Customer endpoints are used to manage customer master data.

### Get all customers

```http
GET /api/customers
```

Returns all non-deleted customers.

#### Response

```http
200 OK
```

---

### Get customer by id

```http
GET /api/customers/{id}
```

Returns a single customer by id.

#### Responses

```http
200 OK
404 Not Found
```

---

### Create customer

```http
POST /api/customers
```

Creates a new customer.

#### Request body

```json
{
  "firstName": "Max",
  "lastName": "Mustermann",
  "company": "Privatkunde",
  "phoneNumber": "0711 123456",
  "email": "max.mustermann@example.com",
  "street": "Gartenstraﬂe",
  "houseNumber": "12",
  "postalCode": "70173",
  "city": "Stuttgart",
  "notes": "Test customer"
}
```

#### Responses

```http
201 Created
400 Bad Request
```

---

### Update customer

```http
PUT /api/customers/{id}
```

Updates an existing customer.

#### Responses

```http
200 OK
400 Bad Request
404 Not Found
```

---

### Delete customer

```http
DELETE /api/customers/{id}
```

Soft deletes an existing customer.

#### Responses

```http
204 No Content
404 Not Found
```

---

## Offered Services

Offered service endpoints are used to manage services that can later be used inside offers.

Examples:

* Lawn mowing
* Hedge cutting
* Green waste disposal
* Pressure washing

---

### Get all offered services

```http
GET /api/offered-services
```

Returns all active/non-deleted offered services.

#### Response

```http
200 OK
```

---

### Get offered service by id

```http
GET /api/offered-services/{id}
```

Returns a single offered service by id.

#### Responses

```http
200 OK
404 Not Found
```

---

### Create offered service

```http
POST /api/offered-services
```

Creates a new offered service.

#### Request body

```json
{
  "name": "Rasen m‰hen",
  "description": "Mowing lawn areas based on square meters.",
  "unit": "m≤",
  "basePrice": 0.18,
  "isActive": true
}
```

#### Responses

```http
201 Created
400 Bad Request
```

---

### Update offered service

```http
PUT /api/offered-services/{id}
```

Updates an existing offered service.

#### Responses

```http
200 OK
400 Bad Request
404 Not Found
```

---

### Delete offered service

```http
DELETE /api/offered-services/{id}
```

Soft deletes an existing offered service.

#### Responses

```http
204 No Content
404 Not Found
```

---

## Offers

Offer endpoints are used to create and manage customer offers.

An offer belongs to one customer and can contain multiple offer items.

---

### Get all offers

```http
GET /api/offers
```

Returns all non-deleted offers.

#### Response

```http
200 OK
```

---

### Get offer by id

```http
GET /api/offers/{id}
```

Returns a single offer by id.

#### Responses

```http
200 OK
404 Not Found
```

---

### Create offer

```http
POST /api/offers
```

Creates a new offer for an existing customer.

The offer number, status and total value are generated by the backend.

#### Request body

```json
{
  "customerId": "00000000-0000-0000-0000-000000000000",
  "validUntil": "2026-12-31T00:00:00Z",
  "notes": "Test offer for garden maintenance."
}
```

#### Server-side behavior

* Verifies that the customer exists
* Generates an offer number
* Sets the initial status to `Draft`
* Sets the initial total value to `0`

#### Responses

```http
201 Created
400 Bad Request
404 Not Found
```

---

### Update offer

```http
PUT /api/offers/{id}
```

Updates an existing offer.

#### Responses

```http
200 OK
400 Bad Request
404 Not Found
```

---

### Delete offer

```http
DELETE /api/offers/{id}
```

Soft deletes an existing offer.

#### Responses

```http
204 No Content
404 Not Found
```

---

## Offer Items

Offer item endpoints are used to manage service positions within existing offers.

An offer item belongs to exactly one offer and references one offered service.
The offered service provides the base price and unit, while the offer item stores the selected quantity, unit price and calculated total price.

---

### Add offer item to offer

```http
POST /api/offers/{offerId}/items
```

Adds a new item to an existing offer.

#### Request body

```json
{
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "quantity": 250
}
```

#### Server-side behavior

* Loads the offer including existing offer items
* Loads the selected offered service
* Uses the offered service base price as the item unit price
* Calculates the item total price using `quantity * unitPrice`
* Adds the item to the offer
* Recalculates the offer total value from all active offer items

#### Example calculation

```text
Quantity: 250 m≤
Unit price: 0.18 Ä
Total price: 45.00 Ä
```

#### Responses

```http
201 Created
400 Bad Request
404 Not Found
500 Internal Server Error
```

#### Example response

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "offerId": "00000000-0000-0000-0000-000000000000",
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "description": "Rasen m‰hen",
  "quantity": 250,
  "unit": "m≤",
  "unitPrice": 0.18,
  "totalPrice": 45.00
}
```

---

### Get offer items by offer

```http
GET /api/offers/{offerId}/items
```

Returns all active offer items for an existing offer.

Soft-deleted offer items are not included in the response.

#### Server-side behavior

* Loads the offer including its offer items
* Filters out soft-deleted offer items
* Returns the active offer items as DTOs

#### Responses

```http
200 OK
404 Not Found
500 Internal Server Error
```

#### Example response

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "offerId": "00000000-0000-0000-0000-000000000000",
    "offeredServiceId": "00000000-0000-0000-0000-000000000000",
    "description": "Rasen m‰hen",
    "quantity": 250,
    "unit": "m≤",
    "unitPrice": 0.18,
    "totalPrice": 45.00
  }
]
```

---

### Update offer item

```http
PUT /api/offers/{offerId}/items/{itemId}
```

Updates the quantity of an existing offer item and recalculates its total price.

#### Request body

```json
{
  "quantity": 300
}
```

#### Server-side behavior

* Loads the offer including existing offer items
* Finds the active offer item by ID
* Updates the item quantity
* Recalculates the item total price using `quantity * unitPrice`
* Recalculates the offer total value from all active offer items

#### Example calculation

```text
Quantity: 300 m≤
Unit price: 0.18 Ä
Total price: 54.00 Ä
```

#### Responses

```http
200 OK
400 Bad Request
404 Not Found
500 Internal Server Error
```

#### Example response

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "offerId": "00000000-0000-0000-0000-000000000000",
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "description": "Rasen m‰hen",
  "quantity": 300,
  "unit": "m≤",
  "unitPrice": 0.18,
  "totalPrice": 54.00
}
```

---

### Delete offer item

```http
DELETE /api/offers/{offerId}/items/{itemId}
```

Soft-deletes an existing offer item and recalculates the offer total value.

The item is not physically removed from the database. Instead, it is marked as deleted and excluded from future offer item queries and total calculations.

#### Server-side behavior

* Loads the offer including existing offer items
* Finds the active offer item by ID
* Marks the item as soft-deleted
* Sets the deletion timestamp
* Recalculates the offer total value from all remaining active offer items

#### Responses

```http
204 No Content
404 Not Found
500 Internal Server Error
```


---

## Orders

Order endpoints are used to create and manage customer orders.

An order is created from an accepted offer.
Each offer can only be converted into one order.

---

### Get all orders

```http
GET /api/orders
```

Returns all non-deleted orders.

#### Responses

```http
200 OK
500 Internal Server Error
```

---

### Get order by id

```http
GET /api/orders/{id}
```

Returns a single order by id.

#### Responses

```http
200 OK
404 Not Found
500 Internal Server Error
```

---

### Create order from offer

```http
POST /api/offers/{offerId}/order
```

Creates a new order from an existing accepted offer.

#### Request body

```json
{
  "plannedDate": "2026-08-01T09:00:00Z",
  "notes": "First order created from accepted offer."
}
```

`plannedDate` is optional, but if provided, it must be in the future.

#### Server-side behavior

* Loads the offer by ID
* Verifies that the offer exists
* Verifies that the offer status is `Accepted`
* Checks that no order already exists for the offer
* Creates a new order with status `Planned`
* Copies the `OfferId` and `CustomerId` from the offer
* Stores the optional planned date and notes

#### Responses

```http
201 Created
400 Bad Request
404 Not Found
409 Conflict
500 Internal Server Error
```

#### Conflict cases

```text
Only accepted offers can be converted into orders.
```

```text
An order already exists for this offer.
```

---

### Update order

```http
PUT /api/orders/{id}
```

Updates an existing order.


#### Request body

```json
{
  "status": 2,
  "plannedDate": "2026-08-05T09:00:00Z",
  "notes": "Order is now in progress."
}
```

#### Server-side behavior

* Loads the order by ID
* Updates the order status
* Updates the planned date
* Updates the notes
* Sets `completedAt` automatically when the status is `Completed`
* Clears `completedAt` when the status is changed away from `Completed`

#### Responses

```http
200 OK
400 Bad Request
404 Not Found
500 Internal Server Error
```

#### Example response

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "offerId": "00000000-0000-0000-0000-000000000000",
  "customerId": "00000000-0000-0000-0000-000000000000",
  "status": 2,
  "plannedDate": "2026-08-05T09:00:00Z",
  "completedAt": null,
  "notes": "Order is now in progress."
}
```

---

### Delete order

```http
DELETE /api/orders/{id}
```

Soft-deletes an existing order.

The order is not physically removed from the database. Instead, it is marked as deleted and excluded from future order queries.

#### Server-side behavior

* Loads the order by ID
* Verifies that the order exists
* Marks the order as soft-deleted
* Sets the deletion timestamp
* Persists the updated order state

#### Responses

```http
204 No Content
404 Not Found
500 Internal Server Error
```
---

## Current Limitations

The current API supports authentication foundation, customer management, offered service management, offer management, offer item management and core order management.

Implemented authentication features:

* Register users
* Login users
* Generate JWT tokens
* Validate JWT bearer tokens
* Retrieve the currently authenticated user through `/api/auth/me`
* Protect business endpoints with JWT authentication
* Test JWT-protected endpoints through Swagger authorization

Implemented offer item features:

* Add offer items to offers
* Get offer items by offer
* Update offer item quantities
* Soft-delete offer items
* Recalculate offer totals after item changes

Implemented order features:

* Create orders from accepted offers
* Get all orders
* Get order by id
* Update order status, planned date and notes
* Soft-delete orders

Not implemented yet:

* Authorization with roles and permissions
* User roles such as Admin or Employee
* Refresh tokens
* Password reset flow
* Email confirmation
* Advanced pricing rules
* Tiered pricing
* PDF generation for offers

