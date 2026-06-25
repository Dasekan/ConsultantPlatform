# Consultant Platform

## Description

Consultant Platform is a full-stack web application built with ASP.NET Core, consisting of a REST API and a Razor Pages frontend.

The project demonstrates a layered architecture where the frontend communicates with the API to manage consultant data. Entity Framework Core is used for database access, while the solution follows modern development practices such as the Service Layer pattern, DTOs, and Dependency Injection.

The project was developed with a focus on maintainability, scalability, and separation of concerns.

---

## Features

- Create consultants
- View consultants
- Update consultant information
- Delete consultants
- REST API
- Razor Pages frontend
- Entity Framework Core
- SQL Server database
- DTO-based communication
- Middleware
- Dependency Injection

---

## Technologies

- C#
- .NET 8
- ASP.NET Core Web API
- Razor Pages
- Entity Framework Core
- SQL Server
- REST API
- Dependency Injection
- DTO Pattern
- Service Layer Architecture

---

## Solution Structure

```text
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
```

---

## Architecture

The solution consists of two separate applications.

### API

The API handles:

- CRUD operations
- Database access
- Business logic
- Validation
- JSON communication

### Web

The web application serves as the user interface and communicates with the API using HTTP.

This architecture provides a clear separation between frontend and backend.

---

## Installation

```bash
git clone https://github.com/Dasekan/ConsultantPlatform.git
```

```bash
cd ConsultantPlatform
```

```bash
dotnet restore
```

Run the API

```bash
dotnet run --project ConsultantPlatform.Api
```

Run the Web project

```bash
dotnet run --project ConsultantPlatform.Web
```

---

## Skills Demonstrated

- ASP.NET Core Web API
- Razor Pages
- Entity Framework Core
- SQL Server
- REST API Design
- CRUD Operations
- Dependency Injection
- Middleware
- Layered Architecture
- Object-Oriented Programming

---

## Future Improvements

- JWT Authentication
- Role-based Authorization
- Unit Tests
- Integration Tests
- Docker
- Azure Deployment
- Logging
- CI/CD

---

## Developed by

**Dasekan Allan Karim**
2. Tracking Web App
# Tracking Web App

## Description

Tracking Web App is an ASP.NET Core MVC application developed to collect and analyze user activity on a website.

The application includes a public website and an administrative dashboard where tracked events can be viewed and analyzed. User interactions are sent to a REST API and stored in a SQLite database using Entity Framework Core.

The project demonstrates how website analytics can be implemented without relying on external tracking services.

---

## Features

- User activity tracking
- REST API
- SQLite database
- Entity Framework Core
- Cookie Authentication
- Admin Dashboard
- Website analytics
- MVC architecture

---

## Technologies

- C#
- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Cookie Authentication
- Bootstrap
- Razor Views

---

## Project Structure

```text
TrackingWebApp
│
├── Controllers
├── Data
├── Entities
├── DTOs
├── Services
├── ViewModels
├── Views
├── wwwroot
└── Program.cs
```

---

## How It Works

1. A user visits the website.
2. JavaScript records user interactions.
3. Events are sent to the REST API.
4. The API validates the data.
5. Events are stored in SQLite.
6. Administrators can review activity through the dashboard.

---

## Installation

```bash
git clone https://github.com/Dasekan/TrackingWebApp.git
```

```bash
cd TrackingWebApp
```

```bash
dotnet restore
```

```bash
dotnet run
```

---

## Skills Demonstrated

- ASP.NET Core MVC
- REST APIs
- Entity Framework Core
- SQLite
- Authentication
- MVC Architecture
- Dependency Injection
- DTO Pattern
- CRUD Operations
- Database Design

---

## Future Improvements

- Analytics Dashboard
- Data Filtering
- CSV Export
- Docker Support
- Unit Tests
- SignalR Live Tracking

---

## Developed by

**Dasekan Allan Karim**
3. Beck Support Bot
# Beck Support Bot

## Description

Beck Support Bot is an ASP.NET Core Web API that provides an AI-powered support assistant.

The application combines OpenAI's GPT models with a local knowledge base to answer questions related to BridgeCentral, BridgeMate, and BC3.

Instead of relying solely on an LLM, the application first searches a local knowledge base for relevant documentation and provides this context to the AI, resulting in more accurate and reliable responses.

---

## Features

- AI-powered support assistant
- OpenAI API integration
- Local knowledge base
- REST API
- Swagger/OpenAPI
- Dependency Injection
- Context-aware AI responses
- Error handling

---

## Technologies

- C#
- .NET 8
- ASP.NET Core Web API
- OpenAI API
- Swagger
- Dependency Injection
- JSON
- REST API

---

## Project Structure

```text
BeckSupportBot
│
├── Controllers
├── Interfaces
├── Services
├── Models
├── KnowledgeBase
├── Program.cs
└── appsettings.json
```

---

## How It Works

1. A user submits a question.
2. The Knowledge Service searches the local documentation.
3. Relevant documents are selected as context.
4. Context is sent together with the user's question to OpenAI.
5. GPT generates a response.
6. The API returns the response as JSON.

---

## Installation

```bash
git clone https://github.com/Dasekan/BeckSupportBot.git
```

```bash
cd BeckSupportBot
```

```bash
dotnet restore
```

Configure your OpenAI API key.

```bash
dotnet run
```

Open Swagger:

```
https://localhost:xxxx/swagger
```

---

## Skills Demonstrated

- ASP.NET Core Web API
- REST API Development
- OpenAI API Integration
- AI-assisted Software Development
- Dependency Injection
- Object-Oriented Programming
- JSON Communication
- Knowledge Retrieval
- Error Handling

---

## Future Improvements

- Semantic Search
- Vector Database
- Docker
- Unit Tests
- Caching
- Authentication
- Logging

---

## Developed by

**Dasekan Allan Karim**
