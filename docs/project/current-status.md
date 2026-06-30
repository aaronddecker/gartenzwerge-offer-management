# Current Project Status

## Overview

Gartenzwerge Außenservice is currently in active development as a full-stack business management application for a landscaping and outdoor service business.

The application already supports the core workflow from customer management to offer creation and order conversion.

```text
Customer
→ Offer
→ Offer Items
→ Accepted Offer
→ Order
```

The project is developed in small, versioned milestones with a focus on realistic business workflows, clean architecture, role-based authorization and a mobile-first React frontend.

---

## Current Milestone

```text
v0.13.0 – Offer Acceptance and Order Conversion UI
```

The current milestone focuses on turning accepted offers into real service orders and separating offer work from operational order handling.

## Current Business Workflow

```mermaid
flowchart LR
    Customer[Customer] --> Offer[Offer]
    Offer --> OfferItems[Offer Items]
    OfferItems --> AcceptedOffer[Accepted Offer]
    AcceptedOffer --> Order[Order]
    Order --> OrderDetails[Read-only Order Details]

    Offer --> OfferArchive[Offer Archive]
    OfferArchive --> Order
```

## Capability Overview

| Area | Backend | Frontend | Status |
|---|---:|---:|---|
| Customer Management | ✅ | ✅ | Usable |
| Offered Services | ✅ | 🟡 Read/Create | Partly usable |
| Offer Creation | ✅ | ✅ | Usable |
| Offer Items | ✅ | ✅ Create/View | Usable |
| Offer Acceptance | ✅ | ✅ | Usable |
| Order Creation | ✅ | ✅ | Usable |
| Orders Overview | ✅ | ✅ | Usable |
| Order Details | ✅ | ✅ Read-only | Read-only |
| Dashboard | 🟡 | 🟡 Placeholder | Planned |
| Analytics | ⏭ | 🟡 Placeholder | Planned |

---

## Current Application Capabilities

### Backend

The backend currently provides:

* Customer management
* Offered service management
* Offer management
* Offer item management
* Order management foundation
* Authentication with ASP.NET Core Identity
* JWT-based authentication
* Role-based authorization
* Admin and Employee roles
* PostgreSQL persistence with Entity Framework Core
* Request validation with FluentValidation
* Global exception handling
* Serilog logging
* Swagger/OpenAPI documentation
* Unit tests for core business logic

### Frontend

The frontend currently provides:

* Login UI connected to the backend authentication API
* JWT token storage
* Protected frontend routes
* Role-aware navigation
* Admin-only frontend areas
* Customer management UI
* Offered service creation UI
* Offer overview
* Offer creation workflow
* Customer lookup during offer creation
* New customer creation inside the offer workflow
* Offer details view
* Offer item creation
* Offer acceptance and order conversion
* Orders overview with real backend data
* Read-only order detail view
* Offer filters for open offers, archived offers and all offers
* Structured mobile-first CSS architecture

---

## Offer and Order Workflow

The current frontend workflow separates offer work from order work:

```text
/offers
→ active offer work and offer history

/orders
→ real service orders
```

Open offers are shown by default under `/offers`.

Converted or rejected offers are available through the archive filter. Converted offers remain available as historical records and link to the related order.

Orders are shown under `/orders` and can be opened through a read-only order detail page.

---

## Current Frontend Routes

```text
/login
/dashboard
/customers
/offers
/offers/new
/offers/:offerId
/orders
/orders/:orderId
/more
/analytics
/offered-services
```

---

## Current Limitations

The project is not yet a finished business application. The following limitations are known:

* No global `AuthContext` yet
* No refresh token flow yet
* Dashboard data is still mostly placeholder-based
* Analytics are not connected to real business data yet
* Orders are currently read-only in the frontend
* Order planning is not editable in the frontend yet
* Order status updates are not editable in the frontend yet
* Order notes are not editable in the frontend yet
* Offered services currently support read and create in the frontend
* Customer lookup during offer creation is currently performed client-side after loading all customers
* The Customers page still contains a visible creation form and may later be refined into a clearer master data area
* `OrderDto` is currently lightweight, so some order overview data is combined from `OrderDto` and related `OfferDto` data in the frontend

---

## Next Planned Step

The next logical milestone is:

```text
v0.14.0 – Order Planning and Status Management UI
```

Planned focus:

* edit order planned date
* update order status
* update order notes
* improve order lifecycle visibility
* prepare upcoming order data for the dashboard

---

## Development Approach

The project follows a small-step development workflow:

* one focused feature per commit
* Conventional Commits
* feature branches through `develop`
* pull requests into `main`
* frontend build and lint checks before committing
* backend tests when backend logic changes
* manual browser testing for frontend workflows

This keeps the project understandable, testable and portfolio-ready.
