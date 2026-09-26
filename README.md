# GymManagementApi (Exam-Ready Backend)

A clean, simple, and easy-to-understand **C# / .NET 10** REST API backend for a gym management system, designed specifically for an exam.

---

## Simple Project Structure (Single Project)

The project has been kept intentionally straightforward with **around 15–20 source files** and **no unnecessary enterprise layers or abstractions**:

```text
GymManagementApi/
│
├── Controllers/
│   ├── AuthController.cs          # Register & Login with JWT
│   ├── TrainersController.cs      # CRUD for Trainers (Admin only for modifications)
│   ├── ExercisesController.cs     # Search, filter, CRUD for Exercises (Admin only for modifications)
│   ├── WorkoutPlansController.cs  # Workout plans with exercises (Trainer/Admin)
│   └── MembershipsController.cs   # Membership management & active rule checks
│
├── Models/
│   ├── User.cs                    # User entity & UserRole enum (Admin, User, Trainer)
│   ├── Trainer.cs                 # Trainer entity
│   ├── Exercise.cs                # Exercise entity
│   ├── WorkoutPlan.cs             # WorkoutPlan entity
│   ├── WorkoutExercise.cs         # Many-to-many join entity (Plan <-> Exercise)
│   └── Membership.cs              # Membership entity & enums (Type, Status)
│
├── DTOs/
│   └── DTOs.cs                    # All request/response DTOs in one clear file
│
├── Validators/
│   └── Validators.cs              # All FluentValidation validators in one file
│
├── Migrations/                    # EF Core database migrations
│   └── ...
│
├── AppDbContext.cs                # EF Core PostgreSQL database context
├── JwtService.cs                  # JWT token generator
├── Program.cs                     # Startup configuration, middleware, Swagger, DB seeding
├── appsettings.json               # Database connection string and JWT settings
├── Dockerfile                     # Multi-stage Docker container build
└── docker-compose.yml             # Runs API and PostgreSQL together
```

---

## Technologies Used

* **Language**: C# (.NET 10)
* **Framework**: ASP.NET Core Web API
* **Database & ORM**: PostgreSQL with Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Authentication**: JWT Bearer Authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`)
* **Password Hashing**: BCrypt (`BCrypt.Net-Next`)
* **Validation**: FluentValidation (`FluentValidation.DependencyInjectionExtensions`)
* **API Documentation**: Swagger / OpenAPI with JWT Authorization (`Swashbuckle.AspNetCore`)
* **Containerization**: Docker & Docker Compose

---

## Quick Start with Docker Compose

To start both **PostgreSQL** and **GymManagementApi**:

```bash
docker compose up -d --build
```

- **Swagger UI**: [http://localhost:5100](http://localhost:5100) (accessible directly at `/`)
- **API URL**: `http://localhost:5100/api`

The database migrations and initial seed data are applied automatically on startup!

To stop containers:

```bash
docker compose down -v
```

---

## Pre-seeded Accounts (For Exam Testing)

| Role | Email | Password |
|---|---|---|
| **Admin** | `admin@gym.com` | `Admin123!` |
| **Trainer** | `trainer@gym.com` | `Trainer123!` |
| **User** | `user@gym.com` | `User123!` |

---

## Implemented Business Rules

1. **Unique Email**: User registration verifies email uniqueness (`409 Conflict`).
2. **Active Membership Constraint**: A user cannot have two active memberships simultaneously (`400 Bad Request`).
3. **Automatic Membership Expiration**: A membership automatically becomes `Expired` when `EndDate` has passed.
4. **Workout Plan Requirements**: A workout plan must contain at least one exercise (`400 Bad Request`).
5. **No Duplicate Exercises**: The same exercise cannot be added twice to the same workout plan (`400 Bad Request`).
6. **Trainer Management RBAC**: Only `Admin` can create, update, and delete trainers (`403 Forbidden` for non-admins).
7. **Exercise Management RBAC**: Only `Admin` can create, update, and delete exercises (`403 Forbidden` for non-admins).
8. **Workout Plan Management**: A `Trainer` (and `Admin`) can create and modify workout plans.
9. **Workout Plan Viewing**: A `User` can view workout plans but cannot create or modify them (`403 Forbidden`).
10. **Membership Creation Ownership**: A regular user can only create a membership for themselves (`403 Forbidden`).

---

## Global Error Handling & Validation Format

All errors return a consistent, clean JSON structure without leaking internal exceptions or stack traces:

```json
{
  "statusCode": 400,
  "message": "User already has an active membership"
}
```

---

## Testing Protected Endpoints in Swagger

1. Navigate to `http://localhost:5100` in your browser.
2. Execute `POST /api/auth/login` with `admin@gym.com` and `Admin123!`.
3. Copy the returned `token`.
4. Click the green **Authorize** button at the top right of Swagger UI.
5. Paste the token into the value box and click **Authorize**.
6. Now you can test all protected endpoints directly.
