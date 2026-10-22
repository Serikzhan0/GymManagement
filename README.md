# APEX Gym Management System (Backend & SPA Frontend)

Современная экосистема для фитнес-клуба на **.NET 10** и **PostgreSQL**, построенная по принципам **Clean Architecture** и **SOLID**, с разделением на проекты в папке `src/` (`Domain`, `Application`, `Persistence`, `Api`), JWT-авторизацией, кошельком, турникетом (СКУД) и встроенным SPA веб-интерфейсом.

---

## Архитектура решения (`src/`)

Решение разделено на независимые проекты в папке `src/` строго по принципам Clean Architecture и SOLID:

```text
GymManagement/
├── .gitignore
├── Dockerfile
├── docker-compose.yml
├── GymManagement.sln
├── README.md
│
└── src/
    ├── GymManagement.Domain/                 # Сущности и базовые доменные типы
    │   ├── Entities/
    │   │   ├── User.cs
    │   │   ├── Trainer.cs
    │   │   ├── Membership.cs
    │   │   ├── Exercise.cs
    │   │   ├── WorkoutPlan.cs
    │   │   ├── WorkoutExercise.cs
    │   │   ├── CheckIn.cs
    │   │   ├── WalletTransaction.cs
    │   │   ├── UserRole.cs
    │   │   ├── MembershipType.cs
    │   │   └── MembershipStatus.cs
    │   └── GymManagement.Domain.csproj
    │
    ├── GymManagement.Application/            # Бизнес-логика, сервисы, DTO, валидаторы
    │   ├── DTOs/
    │   │   ├── Analytics/
    │   │   ├── Auth/
    │   │   ├── CheckIns/
    │   │   ├── Common/
    │   │   ├── Exercises/
    │   │   ├── Memberships/
    │   │   ├── Trainers/
    │   │   ├── Wallet/
    │   │   └── WorkoutPlans/
    │   ├── Repositories/
    │   │   └── Interfaces/                   # Интерфейсы репозиториев
    │   │       ├── IUserRepository.cs
    │   │       ├── ITrainerRepository.cs
    │   │       ├── IExerciseRepository.cs
    │   │       ├── IMembershipRepository.cs
    │   │       ├── IWorkoutPlanRepository.cs
    │   │       ├── ICheckInRepository.cs
    │   │       └── IWalletRepository.cs
    │   ├── Services/
    │   │   ├── Interfaces/                   # Интерфейсы сервисов
    │   │   │   ├── IAuthService.cs
    │   │   │   ├── ITrainerService.cs
    │   │   │   ├── IExerciseService.cs
    │   │   │   ├── IMembershipService.cs
    │   │   │   ├── IWorkoutPlanService.cs
    │   │   │   ├── ICheckInService.cs
    │   │   │   ├── IWalletService.cs
    │   │   │   ├── IAnalyticsService.cs
    │   │   │   └── IJwtService.cs
    │   │   └── Implementations/              # Реализации бизнес-сервисов
    │   │       ├── AuthService.cs
    │   │       ├── TrainerService.cs
    │   │       ├── ExerciseService.cs
    │   │       ├── MembershipService.cs
    │   │       ├── WorkoutPlanService.cs
    │   │       ├── CheckInService.cs
    │   │       ├── WalletService.cs
    │   │       ├── AnalyticsService.cs
    │   │       └── JwtService.cs
    │   ├── Validators/                       # FluentValidation валидаторы
    │   ├── ServiceResult.cs
    │   └── GymManagement.Application.csproj
    │
    ├── GymManagement.Persistence/            # Доступ к данным и EF Core
    │   ├── AppDbContext.cs                   # PostgreSQL DbContext
    │   ├── DataSeeder.cs                     # Автоматический сидер демо-данных
    │   ├── Repositories/
    │   │   └── Implementations/              # EF Core реализации репозиториев
    │   │       ├── UserRepository.cs
    │   │       ├── TrainerRepository.cs
    │   │       ├── ExerciseRepository.cs
    │   │       ├── MembershipRepository.cs
    │   │       ├── WorkoutPlanRepository.cs
    │   │       ├── CheckInRepository.cs
    │   │       └── WalletRepository.cs
    │   └── GymManagement.Persistence.csproj
    │
    └── GymManagement.Api/                    # REST API точка входа и SPA клиент
        ├── Controllers/                      # Тонкие API-контроллеры
        │   ├── AnalyticsController.cs
        │   ├── AuthController.cs
        │   ├── CheckInsController.cs
        │   ├── ExercisesController.cs
        │   ├── MembershipsController.cs
        │   ├── TrainersController.cs
        │   ├── WalletController.cs
        │   └── WorkoutPlansController.cs
        ├── Program.cs                        # DI регистрация, Middleware
        ├── appsettings.json
        ├── wwwroot/                          # Веб-клиент (SPA с личными кабинетами)
        └── GymManagement.Api.csproj
```

---

## Стек технологий

* **Backend**: C# (.NET 10), ASP.NET Core Web API
* **Архитектура**: Clean Architecture + SOLID
* **База данных**: PostgreSQL 16 + Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Безопасность**: JWT Bearer Tokens, BCrypt пароли
* **Валидация**: FluentValidation
* **Документация**: Swagger / OpenAPI
* **Контейнеризация**: Docker, Docker Compose
* **Frontend**: HTML5, Modern CSS, Vanilla JS, Lucide Icons (встроены в `wwwroot`)

---

## Быстрый запуск с Docker

```bash
docker compose up -d --build
```

API и фронтенд будут доступны по адресу: **http://localhost:5100**  
Swagger UI: **http://localhost:5100/swagger**

---

## Тестовые учетные записи

| Роль | Email | Пароль |
| :--- | :--- | :--- |
| **Admin** | `admin@gym.com` | `Admin123!` |
| **Trainer** | `trainer@gym.com` | `Trainer123!` |
| **User** | `user@gym.com` | `User123!` |
