# APEX Gym Management System (Backend & SPA Frontend)

Современная экосистема для фитнес-клуба на **.NET 10** и **PostgreSQL**, построенная по принципам **SOLID**, с разделением на слои (DTO, Repositories, Services, Controllers), JWT-авторизацией, кошельком, турникетом (СКУД) и встроенным SPA веб-интерфейсом.

---

## Архитектура и принципы SOLID

Проект спроектирован с соблюдением принципов чистой архитектуры:
- **S (Single Responsibility Principle)**: Каждый класс и интерфейс вынесен в отдельный файл с единственной ответственностью.
- **O (Open/Closed Principle)**: Расширение функционала через интерфейсы без изменения существующего кода.
- **L (Liskov Substitution Principle)**: Полноценная взаимозаменяемость реализаций репозиториев и сервисов.
- **I (Interface Segregation Principle)**: Узкоспециализированные интерфейсы для каждой доменной области.
- **D (Dependency Inversion Principle)**: Контроллеры и сервисы зависят строго от интерфейсов (IoC/DI).

```text
GymManagement/
├── Controllers/                 # Тонкие API-контроллеры (HTTP routing, status codes)
│   ├── AnalyticsController.cs
│   ├── AuthController.cs
│   ├── CheckInsController.cs
│   ├── ExercisesController.cs
│   ├── MembershipsController.cs
│   ├── TrainersController.cs
│   ├── WalletController.cs
│   └── WorkoutPlansController.cs
│
├── Services/                    # Слой бизнес-логики
│   ├── Interfaces/
│   │   ├── IAnalyticsService.cs
│   │   ├── IAuthService.cs
│   │   ├── ICheckInService.cs
│   │   ├── IExerciseService.cs
│   │   ├── IJwtService.cs
│   │   ├── IMembershipService.cs
│   │   ├── ITrainerService.cs
│   │   ├── IWalletService.cs
│   │   └── IWorkoutPlanService.cs
│   └── Implementations/
│       ├── AnalyticsService.cs
│       ├── AuthService.cs
│       ├── CheckInService.cs
│       ├── ExerciseService.cs
│       ├── JwtService.cs
│       ├── MembershipService.cs
│       ├── TrainerService.cs
│       ├── WalletService.cs
│       └── WorkoutPlanService.cs
│
├── Repositories/                # Слой доступа к данным (EF Core queries)
│   ├── Interfaces/
│   │   ├── ICheckInRepository.cs
│   │   ├── IExerciseRepository.cs
│   │   ├── IMembershipRepository.cs
│   │   ├── ITrainerRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IWalletRepository.cs
│   │   └── IWorkoutPlanRepository.cs
│   └── Implementations/
│       ├── CheckInRepository.cs
│       ├── ExerciseRepository.cs
│       ├── MembershipRepository.cs
│       ├── TrainerRepository.cs
│       ├── UserRepository.cs
│       ├── WalletRepository.cs
│       └── WorkoutPlanRepository.cs
│
├── Models/                      # Сущности базы данных
│   ├── CheckIn.cs
│   ├── Exercise.cs
│   ├── Membership.cs
│   ├── MembershipStatus.cs
│   ├── MembershipType.cs
│   ├── Trainer.cs
│   ├── User.cs
│   ├── UserRole.cs
│   ├── WalletTransaction.cs
│   ├── WorkoutExercise.cs
│   └── WorkoutPlan.cs
│
├── DTOs/                        # Data Transfer Objects по доменам
│   ├── Analytics/
│   ├── Auth/
│   ├── CheckIns/
│   ├── Common/
│   ├── Exercises/
│   ├── Memberships/
│   ├── Trainers/
│   ├── Wallet/
│   └── WorkoutPlans/
│
├── Validators/                  # FluentValidation валидаторы
├── wwwroot/                     # Веб-клиент (SPA с личными кабинетами)
├── AppDbContext.cs              # EF Core PostgreSQL DbContext
├── DataSeeder.cs                # Автоматический сидер демо-данных
├── Program.cs                   # Точка входа, DI регистрация, Middleware
├── Dockerfile                   # Multi-stage Dockerfile
└── docker-compose.yml           # Запуск API и PostgreSQL в Docker
```

---

## Стек технологий

* **Backend**: C# (.NET 10), ASP.NET Core Web API
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
