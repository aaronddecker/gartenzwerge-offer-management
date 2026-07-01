# Local Full-Stack Setup

This guide walks through running the complete Gartenzwerge Außenservice application
locally, from a fresh clone to a working login in the browser.

The stack has three parts that run at the same time:

```text
PostgreSQL (Docker)  →  ASP.NET Core API  →  React Frontend (Vite)
   :5432                    :5041                  :5173
```

---

## Prerequisites

| Tool           | Version | Notes                                   |
| -------------- | ------- | --------------------------------------- |
| .NET SDK       | 9.0     | `dotnet --version`                      |
| Node.js        | 20+     | `node --version`                        |
| npm            | 10+     | ships with Node.js                      |
| Docker Desktop | current | provides the local PostgreSQL container |
| dotnet-ef      | 9.0     | EF Core CLI, see below                  |

Install the EF Core CLI once (used to apply database migrations):

```bash
dotnet tool install --global dotnet-ef
```

---

## Ports and URLs

| Service         | URL                              | Notes                                    |
| --------------- | -------------------------------- | ---------------------------------------- |
| PostgreSQL      | `localhost:5432`                 | container `gartenzwerge-postgres`        |
| API (HTTP)      | `http://localhost:5041`          | default `http` launch profile            |
| API Swagger     | `http://localhost:5041/swagger`  | only in the `Development` environment    |
| Frontend        | `http://localhost:5173`          | Vite dev server                          |

> **Run the API on the HTTP profile.** The frontend calls `http://localhost:5041`
> by default and the API only allows CORS from `http://localhost:5173`. The default
> `dotnet run` uses the `http` profile, so the origins line up out of the box. Using
> the `https` profile (`:7261`) redirects API calls to HTTPS and requires a trusted
> dev certificate — avoid it unless you configure the frontend and cert accordingly.

---

## Step 1 – Start PostgreSQL

From the repository root:

```bash
docker compose up -d
```

This starts a `postgres:16` container named `gartenzwerge-postgres` with:

```text
Database: gartenzwerge_db
User:     postgres
Password: postgres
Port:     5432
```

Data is stored in the named volume `gartenzwerge_postgres_data`, so it survives
container restarts. Check that it is running:

```bash
docker compose ps
```

---

## Step 2 – Apply Database Migrations

The API reads its connection string from
`src/Backend/Gartenzwerge.API/appsettings.Development.json`, which already points at
the container above. Apply the migrations:

```bash
cd src/Backend

dotnet ef database update \
  --project Gartenzwerge.Infrastructure \
  --startup-project Gartenzwerge.API
```

This creates the business tables and the ASP.NET Core Identity tables.

---

## Step 3 – Run the Backend API

```bash
cd src/Backend

dotnet run --project Gartenzwerge.API
```

On startup the API seeds the roles and the local development users (see below), then
listens on `http://localhost:5041`. Open Swagger to confirm it is up:

```text
http://localhost:5041/swagger
```

---

## Step 4 – Run the Frontend

In a second terminal:

```bash
cd src/Frontend

npm install
npm run dev
```

Open the app:

```text
http://localhost:5173
```

By default the frontend targets `http://localhost:5041`. To point it at a different
API, create `src/Frontend/.env.local`:

```dotenv
VITE_API_BASE_URL=http://localhost:5041
```

---

## Step 5 – Log In

The API seeds two development users on startup:

| Email                      | Password   | Role     |
| -------------------------- | ---------- | -------- |
| `test@gartenzwerge.de`     | `Test1234` | Admin    |
| `employee@gartenzwerge.de` | `Test1234` | Employee |

Log in with the Admin user to see every area, including the Admin-only analytics
page. The Employee user sees the operational areas without the Admin sections.

---

## Everyday Commands

Once set up, a normal working session is:

```bash
# terminal 1 – database (only if not already running)
docker compose up -d

# terminal 2 – backend
dotnet run --project src/Backend/Gartenzwerge.API

# terminal 3 – frontend
npm --prefix src/Frontend run dev
```

Quality checks before committing:

```bash
# backend tests
dotnet test src/Backend/Gartenzwerge.sln

# frontend build and lint
npm --prefix src/Frontend run build
npm --prefix src/Frontend run lint
```

On Windows PowerShell, use `npm.cmd` instead of `npm` if `npm` is not on the path.

---

## Troubleshooting

| Symptom                                              | Likely cause and fix                                                                                                   |
| ---------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| Login fails with a CORS or network error             | API is running on the `https` profile. Restart it with the default `http` profile so it serves `http://localhost:5041`. |
| API cannot connect to the database                   | The PostgreSQL container is not running. Run `docker compose up -d` and check `docker compose ps`.                      |
| `relation "..." does not exist` errors from the API  | Migrations were not applied. Run the `dotnet ef database update` command from Step 2.                                   |
| `dotnet ef` is not recognized                        | The EF Core CLI is missing. Run `dotnet tool install --global dotnet-ef`.                                              |
| Port 5432 is already in use                          | Another PostgreSQL instance is running. Stop it, or change the host port mapping in `docker-compose.yml`.               |
| Frontend loads but every request is unauthorized     | The seeded users were not created. Check the API startup logs and confirm the database is reachable.                   |

### Reset the Database

To start from a clean database, remove the container and its volume, then re-apply
the migrations:

```bash
docker compose down -v
docker compose up -d
dotnet ef database update \
  --project src/Backend/Gartenzwerge.Infrastructure \
  --startup-project src/Backend/Gartenzwerge.API
```
