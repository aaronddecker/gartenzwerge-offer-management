# Add Offer Item Flow

This document describes the business and technical flow for adding a new offer item to an existing offer.

The feature is exposed through the following API endpoint:

```http
POST /api/offers/{offerId}/items
```

---

## Purpose

An offer item represents a single service position inside an offer.

Examples:

* Lawn mowing
* Hedge cutting
* Green waste disposal
* Pressure washing

An offer can contain multiple offer items.
The total value of the offer is calculated from the sum of all active offer items.

---

## Business Rule

The client is only allowed to send the selected offered service and the quantity.

The backend is responsible for:

* Loading the selected offered service
* Using the stored base price as the unit price
* Calculating the item total price
* Adding the item to the offer
* Recalculating the offer total value

This prevents clients from manipulating prices directly.

---

## Request

```http
POST /api/offers/{offerId}/items
```

### Route parameter

```text
offerId
```

The id of the offer to which the new item should be added.

### Request body

```json
{
  "offeredServiceId": "00000000-0000-0000-0000-000000000000",
  "quantity": 250
}
```

---

## Flow Diagram

```mermaid
flowchart TD
    A[Client sends POST request] --> B[OfferItemsController]
    B --> C[OfferItemService.AddItemAsync]

    C --> D[Load Offer with existing Items]
    D --> E{Offer found?}

    E -->|No| F[Return 404 Not Found]
    E -->|Yes| G[Load OfferedService]

    G --> H{OfferedService found?}
    H -->|No| F
    H -->|Yes| I[Read BasePrice and Unit from OfferedService]

    I --> J[Calculate TotalPrice = Quantity * UnitPrice]
    J --> K[Create new OfferItem entity]
    K --> L[Add OfferItem to Offer.Items]
    L --> M[Recalculate Offer.TotalNet from all active items]
    M --> N[Save changes using OfferRepository]
    N --> O[Return 201 Created with OfferItemDto]
```

---

## Technical Flow

### 1. Controller receives the request

The `OfferItemsController` receives:

* `offerId` from the URL
* `CreateOfferItemRequest` from the request body

The controller does not contain business logic.
It forwards the request to the application service.

```text
OfferItemsController
↓
IOfferItemService
```

---

### 2. Service loads the offer

The `OfferItemService` loads the offer including existing offer items.

This is necessary because the offer total must be recalculated from all active items.

```text
IOfferRepository.GetByIdWithItemsAsync(offerId)
```

If the offer does not exist, the service returns `null`.

The controller maps this result to:

```http
404 Not Found
```

---

### 3. Service loads the offered service

The selected offered service is loaded by its id.

```text
IOfferedServiceRepository.GetByIdAsync(offeredServiceId)
```

The offered service provides the server-side price information:

* Name
* Unit
* BasePrice

The client does not send `UnitPrice` or `TotalPrice`.

---

### 4. Service calculates the item price

The item price is calculated by the backend.

```text
UnitPrice = OfferedService.BasePrice
TotalPrice = Quantity * UnitPrice
```

Example:

```text
Quantity:   250 m²
UnitPrice:  0.18 €
TotalPrice: 45.00 €
```

---

### 5. Service creates the OfferItem entity

The service creates a new `OfferItem` entity using:

* Offer id
* Offered service id
* Description
* Quantity
* Unit price
* Total price

This entity represents the internal business state that will be persisted.

---

### 6. Service adds the item to the offer

The new item is added to the offer's item collection.

```text
Offer.Items.Add(offerItem)
```

This expresses the business relationship:

```text
One Offer has many OfferItems.
One OfferItem belongs to exactly one Offer.
```

---

### 7. Service recalculates the offer total

The offer total is recalculated from all active offer items.

```text
Offer.TotalNet = Sum of all non-deleted OfferItem.TotalPrice values
```

This is safer than adding only the new item price to the old total because future changes such as updates or soft deletes could otherwise lead to inconsistent totals.

---

### 8. Repository saves the changes

The updated offer is saved through the repository.

```text
OfferItemService
↓
IOfferRepository
↓
OfferRepository
↓
AppDbContext
↓
PostgreSQL
```

The application layer does not know Entity Framework Core directly.
Database-specific logic remains inside the infrastructure layer.

---

## Response

If the item was added successfully, the API returns:

```http
201 Created
```

### Example response

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

---

## Error Cases

### Offer not found

If the offer id does not exist:

```http
404 Not Found
```

### Offered service not found

If the offered service id does not exist:

```http
404 Not Found
```

### Invalid request body

If validation fails:

```http
400 Bad Request
```

---

## Design Decisions

### Why is `offerId` part of the URL?

The offer id identifies the parent resource.

```http
POST /api/offers/{offerId}/items
```

This means:

```text
Add a new item to this specific offer.
```

---

### Why does the client not send `UnitPrice` or `TotalPrice`?

Prices are business-critical values.

If the client could send prices directly, a user could manipulate the offer total.

Therefore:

```text
Client sends selection and quantity.
Backend determines price and total.
```

---

### Why is `Offer.TotalNet` recalculated?

The offer total is derived from the offer items.

Recalculating the total from all active items keeps the offer consistent, especially when items are updated or deleted later.

---

## Current Limitations

The current implementation supports adding offer items.

Not implemented yet:

* Listing all items of an offer
* Updating offer items
* Soft deleting offer items
* Advanced pricing rules
* Tiered pricing for different service types
* Separate error responses for offer-not-found and service-not-found
