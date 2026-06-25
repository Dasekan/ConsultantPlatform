Consultant Platform
Beskrivelse

Consultant Platform er en full-stack webapplikation udviklet i ASP.NET Core, bestående af en REST API og et Razor Pages-webinterface.

Projektet demonstrerer en klassisk lagdelt arkitektur, hvor Web-projektet kommunikerer med API'et for at administrere konsulentdata. Løsningen anvender Entity Framework Core til databaseadgang og følger principper som Service Layer, DTO'er og Dependency Injection.

Projektet er udviklet med fokus på god kodearkitektur, vedligeholdelse og adskillelse mellem frontend og backend.

Funktioner
Oprettelse af konsulenter
Visning af konsulenter
Redigering af konsulenter
Sletning af konsulenter
REST API
Razor Pages frontend
Entity Framework Core
SQL-database
DTO-baseret kommunikation
Middleware
Dependency Injection
Teknologier
C#
.NET 8
ASP.NET Core Web API
Razor Pages
Entity Framework Core
SQL Server
REST API
Dependency Injection
DTO Pattern
Repository / Service Layer
Løsningsstruktur
ConsultantPlatform
│
├── ConsultantPlatform.Api
│   ├── Controllers
│   ├── Services
│   ├── Interfaces
│   ├── Data
│   ├── Models
│   ├── DTOs
│   ├── Middleware
│   └── Program.cs
│
├── ConsultantPlatform.Web
│   ├── Pages
│   ├── wwwroot
│   └── Program.cs
│
└── ConsultantPlatform.sln
Arkitektur

Projektet består af to applikationer.

API

API'et håndterer:

CRUD-operationer
Databaseadgang
Forretningslogik
Validering
JSON-kommunikation
Web

Webprojektet fungerer som brugergrænseflade og kommunikerer med API'et via HTTP.

Dette giver en tydelig adskillelse mellem frontend og backend.

Installation

Klon projektet

git clone https://github.com/Dasekan/ConsultantPlatform.git

Gå til projektmappen

cd ConsultantPlatform

Gendan NuGet-pakker

dotnet restore

Kør API-projektet

dotnet run --project ConsultantPlatform.Api

Kør Web-projektet

dotnet run --project ConsultantPlatform.Web
Kompetencer demonstreret

Projektet demonstrerer erfaring med:

ASP.NET Core Web API
Razor Pages
Entity Framework Core
SQL Server
REST API-design
CRUD-operationer
DTO Pattern
Dependency Injection
Middleware
Lagdelt arkitektur
Objektorienteret programmering
Mulige forbedringer
JWT Authentication
Rollebaseret autorisation
Unit Tests
Integration Tests
Docker
Azure Deployment
Logging
CI/CD med GitHub Actions
Udviklet af

Dasekan
