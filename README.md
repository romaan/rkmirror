# Pre-requisites:

- Docker
- .net 8.0 SDK 
- npm 

Used npm for installing azurite

npm i -g azure-functions-core-tools@4 --unsafe-perm true

Initial migration

dotnet ef migrations add InitialCreate --project PsychologistBooking.Infrastructure --startup-project PsychologistBooking.Seeder

# Frontend

Mobile friendly screens

# Backend

Functions  -->  Application  -->  Domain
            ↘
         Infrastructure (implements Domain Interfaces)

/src
│
├── Domain/
│   ├── Entities/            ← Core business entities (e.g., Psychologist)
│   ├── Enums/               ← Business-related enums
│   ├── Interfaces/          ← Domain-level abstractions (e.g., IRepository)
│
├── Application/
│   ├── Dtos/                ← Data transfer objects
│   ├── Interfaces/          ← Use case contracts (e.g., IGetPsychologistsUseCase)
│   ├── Services/            ← Application services (or UseCases)
│
├── Infrastructure/
│   ├── Data/                ← DbContext, configurations
│   ├── Repositories/        ← Implement domain interfaces
│
├── Functions/               ← Azure Functions (entry points)
│   ├── GetPsychologists.cs  ← Minimal logic, delegate to Application layer


# Seeder

# Tests 

# Playwright Tests

# TODO

- Write Azure pipelines for build, test, deploy
- Write terraform for scaffolding azure infrastructure (SQL Server, Azure Function, Hosting static content as blob storage)


Note: Passwords are stored, but ideally I would not save it

Answer to SQL question? 