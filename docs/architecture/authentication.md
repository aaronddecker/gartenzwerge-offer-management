# Authentication Architecture

This document explains how authentication is implemented in the Gartenzwerge backend.

---

## Goal

The authentication foundation allows users to register, log in and receive a JWT token.

The token can then be used to access protected API endpoints.

---

## Technologies Used

The authentication system uses:

* ASP.NET Core Identity
* Entity Framework Core
* PostgreSQL
* JWT Bearer Authentication
* FluentValidation
* Swagger JWT authorization

---

## Main Components

### ApplicationUser

`ApplicationUser` extends ASP.NET Core Identity's `IdentityUser<Guid>`.

It represents an application user inside the Identity system.

Additional project-specific fields can be added here.

Example:

```text
ApplicationUser : IdentityUser<Guid>
```

---

### Auth DTOs

The Application layer defines request and response DTOs.

Examples:

* `RegisterRequest`
* `LoginRequest`
* `AuthResponse`

These DTOs define the input and output of authentication use cases.

---

### IAuthService

The `IAuthService` interface is defined in the Application layer.

It describes what authentication can do:

* Register a user
* Log in a user

The Application layer only knows this abstraction.

It does not know ASP.NET Core Identity or JWT implementation details.

---

### AuthService

The `AuthService` implementation lives in the Infrastructure layer.

It uses technical authentication dependencies such as:

* `UserManager<ApplicationUser>`
* ASP.NET Core Identity
* JWT token generation
* Application configuration

This keeps Identity and JWT details outside the Application layer.

---

### AuthController

The `AuthController` exposes authentication through HTTP endpoints.

Available endpoints:

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
```

The controller stays thin and delegates the actual authentication logic to `IAuthService`.

---

## Request Flow

### Register

```text
Client
 ↓
POST /api/auth/register
 ↓
AuthController
 ↓
RegisterRequestValidator
 ↓
IAuthService.RegisterAsync
 ↓
AuthService
 ↓
UserManager<ApplicationUser>
 ↓
ASP.NET Core Identity tables
 ↓
JWT token response
```

---

### Login

```text
Client
 ↓
POST /api/auth/login
 ↓
AuthController
 ↓
LoginRequestValidator
 ↓
IAuthService.LoginAsync
 ↓
AuthService
 ↓
UserManager<ApplicationUser>
 ↓
Password validation
 ↓
JWT token response
```

---

### Authenticated Request

```text
Client sends request with Authorization header
 ↓
Authorization: Bearer <token>
 ↓
JWT Bearer Authentication middleware
 ↓
Token signature and lifetime are validated
 ↓
[Authorize] endpoint is executed
```

---

## JWT Token

A JWT token contains claims about the authenticated user.

Current claims include:

* User id
* Email
* Display name
* Token id

The token is signed using a secret key from configuration.

This allows the API to verify that the token was created by the server and was not modified.

---

## Security Decisions

### Same login error message

The login endpoint returns the same error message for unknown emails and wrong passwords.

```text
Invalid email or password.
```

This prevents attackers from discovering whether a specific email address exists.

---

### Password hashing

Passwords are not stored as plain text.

ASP.NET Core Identity hashes passwords before storing them in the database.

The application does not manually set `PasswordHash`.

---

### JWT secret

The JWT secret is used to sign tokens.

If someone modifies a token, the signature becomes invalid and the API rejects the request.

---

## Role-based Authorization

The backend uses ASP.NET Core Identity roles for role-based authorization.

Available roles:

* `Admin`
* `Employee`

Roles are seeded during application startup for local development.

Development users are assigned to roles so authorization behavior can be tested locally:

* `test@gartenzwerge.de` → `Admin`
* `employee@gartenzwerge.de` → `Employee`

JWT tokens include role claims so ASP.NET Core can evaluate role-based authorization rules.

Example:

```text
[Authorize(Roles = ApplicationRoles.Admin)]
```

Authorization behavior:

* `401 Unauthorized` means the request is not authenticated
* `403 Forbidden` means the request is authenticated but the user does not have the required role

## Current Limitations

Implemented:

* User registration
* User login
* Password hashing through ASP.NET Core Identity
* JWT token generation
* JWT bearer authentication
* Role claims in JWT tokens
* Admin and Employee roles
* Role seeding for local development
* Development users for local authorization testing
* Protected `/api/auth/me` endpoint
* Protected business endpoints
* Role-based endpoint protection
* Swagger JWT authorization
* Auth request validation

Not implemented yet:

* Customer user role or customer portal
* User management endpoints
* Runtime role assignment through the API
* Authorization policies
* Refresh tokens
* Password reset flow
* Email confirmation

