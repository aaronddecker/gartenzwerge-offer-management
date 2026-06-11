# Entity Relationships

This document describes the main domain entities and their relationships in the Gartenzwerge offer management backend.

---

## Overview

The application currently manages the following core entities:

* Customer
* OfferedService
* Offer
* OfferItem
* Order

All entities inherit from `BaseEntity`, which provides common fields such as:

* `Id`
* `CreatedAt`
* `UpdatedAt`
* `IsDeleted`
* `DeletedAt`

Soft-deleted entities are marked with `IsDeleted = true` instead of being physically removed from the database.

---

## Customer

A customer represents a person or company that can receive offers and orders.

### Relationships

```text
Customer 1 ─── n Offer
Customer 1 ─── n Order
```

A customer can have multiple offers and multiple orders.

---

## OfferedService

An offered service represents a service that the business can provide.

Examples:

* Lawn mowing
* Hedge cutting
* Green waste disposal
* Pressure washing

### Relationships

```text
OfferedService 1 ─── n OfferItem
```

An offered service can be used in many offer items.

The offered service provides the base price and unit, while the offer item stores the selected quantity and calculated prices.

---

## Offer

An offer represents a customer offer.

### Relationships

```text
Customer 1 ─── n Offer
Offer 1 ─── n OfferItem
Offer 1 ─── 0..1 Order
```

An offer belongs to exactly one customer.

An offer can contain multiple offer items.

An offer can optionally be converted into one order after it has been accepted.

### Important Fields

* `OfferNumber`
* `CustomerId`
* `ValidUntil`
* `Status`
* `TotalNet`
* `Notes`

The offer total value is recalculated whenever offer items are added, updated or soft-deleted.

---

## OfferItem

An offer item represents one service position inside an offer.

### Relationships

```text
Offer 1 ─── n OfferItem
OfferedService 1 ─── n OfferItem
```

An offer item belongs to exactly one offer.

An offer item references exactly one offered service.

### Important Fields

* `OfferId`
* `OfferedServiceId`
* `Description`
* `Quantity`
* `UnitPrice`
* `TotalPrice`

The total price is calculated using:

```text
Quantity * UnitPrice
```

Soft-deleted offer items are excluded from offer total calculations.

---

## Order

An order represents a customer order created from an accepted offer.

### Relationships

```text
Offer 1 ─── 0..1 Order
Customer 1 ─── n Order
```

An order belongs to exactly one offer.

An order belongs to exactly one customer.

Each offer can only be converted into one order.

### Important Fields

* `OfferId`
* `CustomerId`
* `Status`
* `PlannedDate`
* `CompletedAt`
* `Notes`

A new order starts with the status `Planned`.

When an order status is changed to `Completed`, `CompletedAt` is set automatically.

When an order is changed away from `Completed`, `CompletedAt` is cleared again.

Orders are soft-deleted instead of physically removed from the database.

---

## Relationship Summary

```text
Customer
 ├── Offers
 │    └── OfferItems
 │         └── OfferedService
 └── Orders
      └── Offer

Offer
 ├── OfferItems
 └── Order
```

---

## Business Rules

* A customer can have multiple offers.
* A customer can have multiple orders.
* An offer can contain multiple offer items.
* An offer item references one offered service.
* An offer can only be converted into an order when its status is `Accepted`.
* Each offer can only have one order.
* Offer totals are calculated from active offer items.
* Soft-deleted records are excluded from normal queries.
