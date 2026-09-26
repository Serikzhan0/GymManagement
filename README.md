# GymManagementApi

A clean, exam-ready backend REST API for gym management built with **C#**, **.NET 10**, **ASP.NET Core Web API**, **Entity Framework Core**, **PostgreSQL**, **JWT Authentication**, **FluentValidation**, and **Docker**.

---

## Architecture

The project follows a clean 4-tier modular architecture adhering to SOLID principles without unnecessary abstractions (no CQRS, MediatR, or microservices):

```text
GymManagementApi
├── GymManagement.Domain            # Entities, Enums, Core domain models
├── GymManagement.Application       # DTOs, FluentValidation validators, Service interfaces, Custom exceptions
├── GymManagement.Infrastructure    # EF Core DbContext, PostgreSQL configs, Migrations, Repositories/Services, Security
└── GymManagement.Api               # Web API Controllers, Middleware, Swagger JWT setup, Program.cs
```

---

## Features & Implemented Business Rules

1. **Active Membership Constraint**: A user cannot have two active memberships at the same time (`400 Bad Request`).
2. **Automatic Membership Expiration**: A membership automatically becomes `Expired` when `EndDate` has passed.
3. **Workout Plan Requirements**: A workout plan must contain at least one exercise (`400 Bad Request`).
4. **No Duplicate Exercises**: The same exercise cannot be added twice to the same workout plan (`400 Bad Request`).
5. **Trainer Management RBAC**: Only an `Admin` can create, update, and delete trainers (`403 Forbidden` for non-admins).
6. **Exercise Management RBAC**: Only an `Admin` can create, update, and delete exercises (`403 Forbidden` for non-admins).
7. **Workout Plan Management**: A `Trainer` (and `Admin`) can create and modify workout plans.
8. **Workout Plan Viewing**: A `User` can view workout plans but cannot create or modify them (`403 Forbidden`).
9. **Membership Authorization**: A user cannot create a membership for another user (`403 Forbidden`).
10. **Unique Email**: User email addresses must be unique (`409 Conflict`).
11. **Password Security**: Passwords are never stored as plain text (hashed with BCrypt).
12. **Global Exception Handling**: Returns consistent JSON error envelopes (`{ "statusCode": ..., "message": "..." }`) and hides internal stack traces.
13. **FluentValidation**: Validates required fields, email format, password length, positive numbers, and date ranges.

---

## Quick Start with Docker Compose

Run the entire application (PostgreSQL + API) with a single command:

```bash
docker compose up -d --build
```

- **Swagger UI**: [http://localhost:5100](http://localhost:5100) (or root `/`)
- **API Base URL**: `http://localhost:5100/api`

The database schema and initial seed data are applied automatically on startup!

To stop the containers:

```bash
docker compose down
```

---

## Pre-seeded Test Accounts

| Role | Email | Password |
|---|---|---|
| **Admin** | `admin@gym.com` | `Admin123!` |
| **Trainer** | `trainer@gym.com` | `Trainer123!` |
| **User** | `user@gym.com` | `User123!` |

---

## Running Locally without Docker

1. Ensure PostgreSQL is running on `localhost:5432` (or adjust connection string in `appsettings.json`).
2. Apply EF Core migrations:
   ```bash
   dotnet ef database update --project GymManagement.Infrastructure --startup-project GymManagement.Api
   ```
3. Run the API:
   ```bash
   dotnet run --project GymManagement.Api
   ```
4. Access Swagger UI at `https://localhost:7198/` or `http://localhost:5000/`.

---

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate and obtain JWT token

### Trainers
- `GET /api/trainers` - List all trainers
- `GET /api/trainers/{id}` - Get trainer by ID
- `POST /api/trainers` - Create a trainer (*Admin only*)
- `PUT /api/trainers/{id}` - Update a trainer (*Admin only*)
- `DELETE /api/trainers/{id}` - Delete a trainer (*Admin only*)

### Exercises
- `GET /api/exercises` - List exercises with optional filters (`?muscleGroup=Chest`, `?search=bench`)
- `GET /api/exercises/{id}` - Get exercise by ID
- `POST /api/exercises` - Create an exercise (*Admin only*)
- `PUT /api/exercises/{id}` - Update an exercise (*Admin only*)
- `DELETE /api/exercises/{id}` - Delete an exercise (*Admin only*)

### Workout Plans
- `GET /api/workout-plans` - List workout plans with exercises
- `GET /api/workout-plans/{id}` - Get workout plan by ID
- `POST /api/workout-plans` - Create workout plan (*Trainer / Admin*)
- `PUT /api/workout-plans/{id}` - Update workout plan (*Trainer / Admin*)
- `DELETE /api/workout-plans/{id}` - Delete workout plan (*Trainer / Admin*)

### Memberships
- `GET /api/memberships` - List memberships (User sees own; Admin sees all)
- `GET /api/memberships/{id}` - Get membership by ID
- `POST /api/memberships` - Create membership (Auto-enforces active membership limits)
- `PUT /api/memberships/{id}` - Update membership
- `DELETE /api/memberships/{id}` - Delete membership

---

## Testing in Swagger

1. Open Swagger UI at `http://localhost:5100`.
2. Call `POST /api/auth/login` with `admin@gym.com` / `Admin123!`.
3. Copy the `token` from the response.
4. Click the **Authorize** button at the top right of Swagger UI.
5. Paste the token into the value box and click **Authorize**.
6. Execute protected endpoints directly from Swagger.
