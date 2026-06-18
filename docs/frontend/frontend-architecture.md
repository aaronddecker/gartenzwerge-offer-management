# Frontend Architecture

This document explains the current frontend foundation of the Gartenzwerge management application.

## Goal

The frontend is built as a React client for the existing ASP.NET Core backend API.

It provides the foundation for a mobile-first business application with routing, a shared app layout, reusable UI components and future API integration.

## Technologies

* React
* TypeScript
* Vite
* React Router
* CSS with mobile-first responsive styling

## Project Structure

```text
src/
├── api/
├── app/
├── pages/
└── shared/
```

## Responsibilities

### app

Contains app-wide structure such as layout and routing-related components.

Examples:

* `AppLayout`

### pages

Contains route-level page components.

Examples:

* `DashboardPage`
* `CustomersPage`
* `OffersPage`
* `OrdersPage`
* `AnalyticsPage`
* `MorePage`
* `LoginPage`

### shared

Contains reusable UI components that can be used across multiple pages.

Examples:

* `PageHeader`
* `StatCard`
* `QuickActionLink`

### api

Reserved for future API configuration and HTTP communication with the ASP.NET Core backend.

This area will later contain API client logic, request helpers and endpoint-specific functions.

## Routing

Current frontend routes:

```text
/dashboard
/customers
/offers
/orders
/more
/analytics
/offered-services
/login
```

## UI/UX Decisions

The frontend follows a mobile-first approach.

On mobile, the primary navigation focuses on the most important work areas:

```text
Dashboard
Customers
Offers
Orders
More
```

Secondary areas such as Analytics and Offered Services are accessible through the More page.

This keeps the main navigation simple and avoids horizontal scrolling on mobile devices.

## Mobile Navigation

The mobile navigation is designed as a bottom navigation.

Reasons:

* Important areas are directly reachable
* No hidden horizontal scrolling
* Better app-like user experience on mobile
* Clear structure for future mobile or PWA usage

The navigation uses safe-area spacing so that browser or device controls do not cover the menu on mobile devices.

## Dashboard vs Analytics

The dashboard is intended as an operational entry point.

It should focus on:

* Quick actions
* Upcoming work
* Important daily overview
* Fast access to core workflows

Analytics is intended for reporting and business insights.

It can later include:

* Customer statistics
* Completed order statistics
* Revenue overview
* Revenue charts
* Offer-to-order conversion insights
* Top services

## Current Frontend Status

Implemented:

* React + TypeScript + Vite setup
* React Router foundation
* Basic app layout
* Mobile-first navigation
* More page
* Analytics page placeholder
* Placeholder pages for core business areas
* Reusable page header component
* Reusable statistic card component
* Reusable quick action link component
* Initial dashboard foundation
* Dashboard quick actions

## Current Limitations

* No backend API integration yet
* No login functionality yet
* No protected frontend routes yet
* No JWT handling in the frontend yet
* No real dashboard data yet
* No real analytics data yet

## Future Improvements

Planned frontend improvements:

* Login form with JWT authentication
* Token storage and auth state handling
* Protected frontend routes
* Load current user through `/api/auth/me`
* Logout functionality
* Role-aware UI behavior for Admin and Employee
* Customer CRUD UI
* Offered Service CRUD UI
* Offer Management UI
* Order Management UI
* Dashboard with upcoming orders
* Calendar field for upcoming orders
* Analytics with real business data
* Loading, error and empty states
* API client abstraction
