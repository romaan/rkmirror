# Psychologist Booking System

A modern full-stack application for booking psychologists, built with Clean Architecture principles on the backend, a Vue frontend, and Playwright end-to-end tests.

# Prerequisites for development

- Docker
- dotnet 8.0 SDK
- npm 10.0+ and node v22.0
- bash shell

# Backend development workflow

The project includes a convenient shell script `dev.sh` to streamline development tasks like starting SQL Server in Docker, running Entity Framework migrations, seeding data, and running the Azure Functions API.

```bash
cd backend
# Start SQL Server, apply migrations, and seed the database
./dev.sh setup
# Run the Azure Function API locally (on port 8000)
./dev.sh run
# In a new window run backend integration tests
./dev.sh test
# Stop and remove the SQL Server Docker container
./dev.sh stop
```

### Configuration Notes
Database Image: Uses mcr.microsoft.com/mssql/server:2022-latest

DB Credentials: Hardcoded in script for local development:
SA_PASSWORD="YourStrongPassword1@"
DB_NAME="PsychologistDb"

EF Context: Uses AppDbContext defined in the Infrastructure project


## Project Structure

```text
.
├── frontend/                  # Nuxt 3 frontend application
│   ├── app.vue                # Root Vue component
│   ├── nuxt.config.ts         # Nuxt app configuration
│   ├── tsconfig.json          # TypeScript configuration
│   ├── components/            # UI components (List, Filter, Card)
│   ├── composables/           # Reusable logic (e.g., fetching psychologists)
│   ├── pages/                 # Nuxt pages (auto-routes)
│   ├── public/                # Static assets (favicon, robots.txt, images)
│   ├── .nuxt/                 # Nuxt build artifacts (auto-generated)
│   ├── .output/               # Nitro output (for server-side rendering)
│   ├── .idea/                 # IDE configs (optional for JetBrains)
│   ├── package.json           # Frontend dependencies and scripts
│   └── README.md              # Docs for the frontend

├── backend/                   # .NET 8 Azure Functions backend with Clean Architecture
│   ├── PsychologistBooking.Application/   # Use cases, DTOs
│   │   ├── Dtos/
│   │   └── UseCases/

│   ├── PsychologistBooking.Domain/        # Entities, Enums, Interfaces
│   │   ├── Enums/
│   │   ├── Entities/
│   │   └── Interfaces/

│   ├── PsychologistBooking.Infrastructure/  # EF Core setup and repositories
│   │   ├── Data/
│   │   │   └── Configurations/
│   │   ├── Repositories/
│   │   └── Migrations/

│   ├── PsychologistBooking.Functions/     # Azure Functions entry points
│   │   ├── GetPsychologists.cs
│   │   ├── GetPsychologistTypes.cs
│   │   ├── local.settings.json            # Local settings (ignored in git)
│   │   ├── Program.cs                     # Function app bootstrap
│   │   └── host.json                      # Azure Functions metadata

│   ├── PsychologistBooking.Seeder/        # Seeder console app for local dev
│   │   ├── Program.cs
│   │   ├── appsettings*.json              # Seeder configuration

│   ├── PsychologistBooking.IntegrationTests/  # Backend integration tests
│   │   ├── Functions/
│   │   └── Helpers/

│   ├── PsychologistBooking.sln            # Solution file
│   └── global.json                        # Global SDK config (e.g., .NET version)

├── tests/                     # End-to-end testing with Playwright
│   ├── e2e/                   # Test suites (e.g., filtering, homepage)
│   ├── fixtures/              # Test setup fixtures
│   ├── pages/                 # Page object models
│   ├── utils/                 # Global setup/teardown, DB helpers
│   ├── types/                 # Shared test types (e.g., Psychologist)
│   ├── playwright.config.ts   # Playwright test runner config
│   ├── test-results/          # Test artifacts (screenshots, logs)
│   ├── playwright-report/     # HTML report
│   ├── .env                   # Environment variables for test runs
│   └── package.json           # Test scripts and dependencies
```

# Backend

The backend is developed using Clean Architecture principles, ensuring a clear separation of concerns across different layers of the application. This structure enhances testability, maintainability, and scalability.

## Layers

- Domain: Contains core business logic, entities, enums, and repository interfaces.
- Application: Defines use case interfaces and DTOs that encapsulate input/output contracts.
- Infrastructure: Implements database access using Entity Framework Core, including repository logic and migration configuration.
- Functions: Serves as the API layer using Azure Functions (Isolated Worker model) to expose HTTP endpoints for clients.

## Entity Framework Core Migrations
The backend uses EF Core migrations to manage and track database schema changes over time. This ensures that changes to entity models are versioned and can be applied or rolled back using standard EF tooling.

To create or apply migrations:

```bash

cd backend

# Create a new migration
dotnet ef migrations add AddInitialSchema --project PsychologistBooking.Infrastructure --startup-project PsychologistBooking.Seeder --context AppDbContext

# Apply migrations to the database
dotnet ef database update --project PsychologistBooking.Infrastructure --startup-project PsychologistBooking.Seeder --context AppDbContext
```

## Integration Tests
The solution includes a dedicated project for integration testing, located at: `/backend/PsychologistBooking.IntegrationTests/`

These tests validate real interactions between the API, database, and domain logic, ensuring that the system behaves as expected under end-to-end scenarios.

# API Hosting
By default, the backend (Azure Functions app) runs on http://localhost:7000 when started locally via:

```bash
cd backend/PsychologistBooking.Functions
func start --port 7001
```

This port can be configured via environment variables or local.settings.json. Further, secrets are left in files like settings.json or dev.sh for demonstration purposes only.

## Seeder Project
The solution includes a dedicated seeder project called:

```bash
/backend/PsychologistBooking.Seeder/
dotnet run
```

This project is designed to generate and insert random sample data into the database. It is especially useful for:

- Local development
- Testing and QA environments
- Populating data for end-to-end testing or UI validation

# Frontend

Vue app is implemented on top of Nuxt framework. Web page is designed to be **mobile friendly**, and has been organised into simple list, card and filter components.

In order to run locally:

```bash
npm install
npm run dev
```

# Playwright Tests

End-to-End Testing with Playwright
The project includes a robust end-to-end test suite built using Playwright and follows the Page Object Model (POM) design pattern to promote test maintainability, reusability, and clarity.

## Test Design: Page Object Model
All Playwright tests are written with the Page Object Model in mind. Each logical page or UI area is abstracted into its own class.

This approach helps:

- Keep test logic clean and focused
- Reuse common interactions across multiple test files
- Simplify maintenance as UI evolves
- Database Seeding & Cleanup: Playwright is integrated with database setup logic to ensure each test run operates on a clean and consistent environment.

The database is automatically seeded with test data at the start of the test run.

The data is cleared after all tests complete, ensuring no residual test state affects other runs.

These operations are handled via test fixtures defined in: `/tests/fixtures/db-fixture.ts`

## How to Run Tests
To run the full end-to-end test suite with the frontend and backend already running, use:

```bash
cd tests
npm run test:chrome
```

This will:

Connect to the running frontend and backend

Execute all Playwright tests in a real Chromium browser

Generate test results (and optionally screenshots or HTML reports)

Ensure your backend is running at http://localhost:7000 and your frontend at http://localhost:3000 before running this command.

## Test Reports and Screenshots
Test artifacts such as screenshots, traces, and reports are available under:

- /tests/test-results/
- /tests/playwright-report/

To view the interactive HTML report:

```bash
npx playwright show-report
```

# TODO

- Write Azure pipelines for build, test, deploy
- Write terraform for scaffolding azure infrastructure (SQL Server, Azure Function, Hosting static content as blob storage)


`Note`: Passwords are stored, but ideally I would not save it

Answer to SQL question "what t-sql query would you write to perform the filtering required to return the psychologist list?":

```sql
SELECT [p0].[Id], [p0].[FirstName], [p0].[LastName], [p0].[PsychologistType], [p0].[ShortDescription], [a].[Id], [a].[Date], [a].[PsychologistId]
 FROM (
     SELECT [p].[Id], [p].[FirstName], [p].[LastName], [p].[PsychologistType], [p].[ShortDescription]
     FROM [Psychologists] AS [p]
     ORDER BY (SELECT 1)
     OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
 ) AS [p0]
 LEFT JOIN [AvailableDates] AS [a] ON [p0].[Id] = [a].[PsychologistId]
 ORDER BY [p0].[Id]
```
