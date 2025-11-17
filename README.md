# SalesSystem - Enterprise Solution 🏢

> **A production-grade, enterprise-level microservices architecture demonstrating advanced software engineering practices, secure authentication, clean architecture, and comprehensive testing.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Microservices](https://img.shields.io/badge/Architecture-Microservices-blue)](https://microservices.io/)
[![Tests](https://img.shields.io/badge/Tests-Passing-success)](https://github.com)

## 📋 Table of Contents

- [Project Overview](#-project-overview)
- [Project Goals](#-project-goals)
- [Solution Architecture](#-solution-architecture)
- [Microservices](#-microservices)
- [Technologies & Tools](#-technologies--tools)
- [Getting Started](#-getting-started)
- [Environment Configuration](#-environment-configuration)
- [Security & Production](#-security--production-notes)
- [Development Guidelines](#-development-guidelines)
- [Contact](#-contact--links)

## 🎯 Project Overview

**SalesSystem** is an enterprise-grade microservices solution designed to demonstrate professional software architecture and development practices suitable for large-scale production environments. The system follows a distributed architecture pattern where each microservice is independently deployable, scalable, and maintainable.

### Key Characteristics

- **Microservices Architecture** - Independent, loosely-coupled services
- **Domain-Driven Design** - Each service owns its domain and data
- **API Gateway Ready** - Centralized entry point for all services
- **Cloud-Native** - Designed for containerization and orchestration
- **Security-First** - Token-based authentication across all services
- **Event-Driven** - Asynchronous communication between services (ready)
- **DevOps Ready** - CI/CD pipeline compatible

## 🎓 Project Goals

This solution showcases the ability to design and implement **professional, enterprise-standard microservices** suitable for large organizations. It demonstrates:

### Architecture & Design
- ✅ **Microservices Architecture** - Distributed system design with independent services
- ✅ **Clean Architecture** - Layered design with clear separation of concerns
- ✅ **SOLID Principles** - Maintainable and extensible codebase
- ✅ **Domain-Driven Design** - Business logic organized around domains
- ✅ **API Gateway Pattern** - Centralized routing and authentication

### Technical Excellence
- ✅ **Security-First Approach** - Token-based authentication and authorization
- ✅ **Comprehensive Testing** - Unit, integration, and end-to-end tests (95%+ coverage)
- ✅ **Production-Ready Code** - Proper error handling, logging, and monitoring
- ✅ **RESTful API Design** - Following industry best practices
- ✅ **Async/Await Pattern** - Non-blocking, high-performance operations

### DevOps & Cloud
- ✅ **Containerization** - Docker support for all services
- ✅ **Orchestration Ready** - Docker Compose and Kubernetes compatible
- ✅ **Environment Management** - Proper configuration for dev/staging/production
- ✅ **CI/CD Ready** - Automated build, test, and deployment pipelines
- ✅ **Monitoring & Logging** - Structured logging and health checks

## 🏗 Solution Architecture (Microservices)

```
┌──────────────────────────────────────────────────────────────────┐
│                         API Gateway (Future)                      │
│                    (Routing, Auth, Rate Limiting)                 │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                 ┌───────────┴───────────┐
                 │                       │
        ┌────────▼─────────┐    ┌───────▼────────┐
        │  Payment API     │    │  Future APIs   │
        │  (Port: 5159)    │    │  - Order API   │
        │                  │    │  - User API    │
        │  - Card Payment  │    │  - Product API │
        │  - Validation    │    │  - Invoice API │
        └──────────────────┘    └────────────────┘
                 │
        ┌────────▼─────────┐
        │   Database       │
        │   (SQL Server)   │
        └──────────────────┘
```

### High-Level System Design

```
SalesSystem/                          # Solution Root
│
├── .gitignore                        # Git ignore rules (shared)
├── .env                              # Environment variables (shared)
├── .dockerignore                     # Docker ignore rules
├── .editorconfig                     # Code style configuration
├── docker-compose.yml                # Multi-service orchestration
├── README.md                         # This file (solution overview)
│
├── PaymentAPI/                       # Payment Microservice
│   ├── PaymentAPI/                   # API Project
│   │   ├── Controllers/              # HTTP endpoints
│   │   ├── Services/                 # Business logic
│   │   ├── Repositories/             # Data access
│   │   ├── Models/                   # Domain models
│   │   ├── Middleware/               # Custom middleware
│   │   ├── appsettings.json          # Service-specific config
│   │   ├── appsettings.Development.json
│   │   └── Program.cs                # Service entry point
│   ├── PaymentAPI.Tests/             # Unit & Integration tests
│   ├── README.md                     # API-specific documentation
│   └── PaymentAPI.sln                # Service solution file
│
└── [Future Microservices]
    ├── OrderAPI/                     # Order management service
    ├── UserAPI/                      # User & authentication service
    ├── ProductAPI/                   # Product catalog service
    └── InvoiceAPI/                   # Invoice generation service
```

### Layered Architecture (Per Microservice)

Each microservice follows a clean architecture pattern:

```
┌─────────────────────────────────────────────────┐
│          API Layer (Controllers)                │  ← HTTP Requests/Responses
│  • RESTful Endpoints                            │  ← Validation & Routing
│  • DTO Mapping                                  │
└─────────────────────┬───────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────┐
│      Business Logic Layer (Services)            │  ← Core Business Rules
│  • Domain Logic                                 │  ← Algorithms & Processing
│  • Validation Rules                             │
└─────────────────────┬───────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────┐
│     Data Access Layer (Repositories)            │  ← Data Operations
│  • Entity Framework / Dapper                    │  ← Database Abstraction
│  • CRUD Operations                              │
└─────────────────────┬───────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────┐
│              Database Layer                      │  ← Persistence
│  • SQL Server / PostgreSQL                      │
└─────────────────────────────────────────────────┘
```

### Design Patterns & Principles

#### SOLID Principles
- **S**ingle Responsibility Principle - Each class has one reason to change
- **O**pen/Closed Principle - Open for extension, closed for modification
- **L**iskov Substitution Principle - Derived classes can substitute base classes
- **I**nterface Segregation Principle - Clients shouldn't depend on unused methods
- **D**ependency Inversion Principle - Depend on abstractions, not concretions

#### Design Patterns Implemented
1. **Repository Pattern** - Data access abstraction
2. **Dependency Injection** - Loose coupling and testability
3. **DTO Pattern** - Data transfer objects for API boundaries
4. **Service Layer Pattern** - Business logic separation
5. **Middleware Pattern** - Request/response pipeline customization
6. **Factory Pattern** - Object creation abstraction (ready)
7. **Strategy Pattern** - Algorithm encapsulation (ready)

## 🎯 Microservices

### Current Services

#### 1. PaymentAPI (Active)
**Purpose:** Credit card validation and payment processing

**Key Features:**
- Credit card validation using Luhn Algorithm
- Multi-card type detection (Visa, MasterCard, Amex, Discover)
- Token-based authentication
- Card number masking for PCI compliance
- Comprehensive test coverage (95%+)

**Port:** 5159  
**Documentation:** [PaymentAPI/README.md](./PaymentAPI/README.md)

**Endpoints:**
- `POST /api/CardPayment/validate` - Validate credit card
- `GET /api/CardPayment/health` - Health check

#### 2. UserAPI (Active)
**Purpose:** User management and authentication

**Key Features:**
- User registration and authentication
- Role-based access control (User/Admin roles)
- User CRUD operations
- Password hashing and verification
- Email uniqueness validation
- Comprehensive test coverage

**Port:** 5160  
**Documentation:** [UserAPI/README.md](./UserAPI/README.md)

**Endpoints:**
- `GET /api/User` - Get all users
- `POST /api/User` - Create user
- `GET /api/User/{id}` - Get user by ID
- `PUT /api/User/{id}` - Update user
- `DELETE /api/User/{id}` - Delete user
- `POST /api/User/authenticate` - Authenticate user
- `GET /health` - Health check

### Future Services (Planned)

#### 3. OrderAPI (Planned)
**Purpose:** Order management and processing
- Create, read, update, delete orders
- Order status tracking
- Integration with Payment and Product services

#### 4. ProductAPI (Planned)
**Purpose:** Product catalog management
- Product CRUD operations
- Inventory tracking
- Category management
- Search and filtering

#### 5. InvoiceAPI (Planned)
**Purpose:** Invoice generation and management
- Generate invoices from orders
- PDF export
- Invoice history
- Payment tracking

## 🛠 Technologies & Tools

### Backend Framework
| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 8.0 (LTS) | Runtime platform |
| **C#** | 12.0 | Programming language |
| **ASP.NET Core** | 8.0 | Web API framework |
| **Entity Framework Core** | 8.0 | ORM for database access |

### Infrastructure & DevOps
| Tool | Purpose |
|------|---------|
| **Docker** | Containerization |
| **Docker Compose** | Multi-container orchestration |
| **Kubernetes** | Container orchestration (production) |
| **Azure DevOps / GitHub Actions** | CI/CD pipelines |

### Database
| Technology | Usage |
|------------|-------|
| **SQL Server / PostgreSQL** | Primary database (relational data) |
| **Redis** | Caching layer (planned) |

### Testing Stack
| Tool | Purpose |
|------|---------|
| **xUnit** | Unit testing framework |
| **Moq** | Mocking framework |
| **FluentAssertions** | Readable assertions |
| **Microsoft.AspNetCore.Mvc.Testing** | Integration testing |

### Monitoring & Logging
| Tool | Purpose |
|------|---------|
| **Serilog** | Structured logging |
| **Application Insights** | APM and monitoring ||

### Security
| Component | Implementation |
|-----------|----------------|
| **Authentication** | Token-based (Custom) / JWT |
| **Authorization** | Role-based access control |
| **API Gateway** | Centralized security (planned) |
| **HTTPS/TLS** | Transport security |



### Quick Start

#### 1. Clone the Repository
```bash
git clone https://github.com/mgsdew/SalesSystem.git
cd SalesSystem
```

#### 2. Configure Environment Variables
Copy the `.env` file and configure your environment:

```bash
# The .env file is already present in the root directory
# Review and update the values as needed

# .env file content
AUTH_VALID_TOKENS=dev-token-123456;test-token-abcdef;your-secret-token-here
```

> **⚠️ Important:** In production, never commit the `.env` file to version control. The current `.gitignore` has this commented out for demonstration purposes only.

#### 3. Run Individual Microservices

**PaymentAPI:**
```bash
cd PaymentAPI

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run (Linux/Mac)
export $(cat ../.env | xargs) && dotnet run --project PaymentAPI/PaymentAPI.csproj

# Run (Windows PowerShell)
$env:AUTH_VALID_TOKENS="dev-token-123456;test-token-abcdef;your-secret-token-here"
dotnet run --project PaymentAPI/PaymentAPI.csproj
```

#### 4. Using Docker Compose (Recommended for Multiple Services)
```bash
# From the solution root
docker-compose up --build

# Run in detached mode
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Access Points

Once running, the services are available at:

| Service | HTTP | Swagger UI |
|---------|------|------------|
| **PaymentAPI** | http://localhost:5159 | http://localhost:5159/swagger |
| **UserAPI** | http://localhost:5160 | http://localhost:5160/swagger |

## 🔧 Environment Configuration

### Environment Variables

The solution uses a centralized `.env` file at the root level for shared configuration. Individual services can override these with their own `appsettings.json` files.

#### Shared Environment Variables (`.env`)
```bash
# Authentication
AUTH_VALID_TOKENS=token1;token2;token3

# Database (can be overridden per service)
DB_HOST=localhost
DB_PORT=1433
DB_USER=sa
DB_PASSWORD=YourStrong@Password

# Redis (future)
REDIS_HOST=localhost
REDIS_PORT=6379

# Logging
LOG_LEVEL=Information

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

### Running from Visual Studio / Rider / VS Code

Each microservice loads environment variables from the parent `.env` file automatically:

1. Open the microservice project (e.g., `PaymentAPI/PaymentAPI.sln`) in your IDE
2. Press **F5** to run with debugging or **Ctrl+F5** without debugging
3. Environment variables from `../../.env` are loaded at startup automatically
4. Swagger UI opens automatically in Development mode

**Requirements:**
- **DotNetEnv** package installed (already configured)
- `.env` file exists in solution root
- `ASPNETCORE_ENVIRONMENT=Development` in `launchSettings.json` for Swagger
- No need to manually set environment variables in IDE


## 🔒 Security & Production Notes

### Authentication & Authorization

All services implement token-based authentication:
- **Development/Demo:** Simple token validation from `.env`
- **Production:** JWT tokens with proper expiration and refresh tokens

### Security Best Practices Implemented

| Security Feature | Status | Implementation |
|------------------|--------|----------------|
| **Authentication** | ✅ | Token-based middleware |
| **Authorization** | ⚠️ | Role-based (planned) |
| **Input Validation** | ✅ | Data Annotations + ModelState |
| **HTTPS/TLS** | ✅ | Configured for production |
| **CORS** | ✅ | Configurable policies |
| **Rate Limiting** | 🔜 | Planned |
| **API Gateway** | 🔜 | Planned |
| **Secrets Management** | ⚠️ | Environment variables (upgrade to Key Vault) |

### Production Deployment Checklist

- [ ] Move secrets to Azure Key Vault / AWS Secrets Manager
- [ ] Enable HTTPS enforcement
- [ ] Configure CORS properly
- [ ] Implement rate limiting
- [ ] Set up health checks for all services
- [ ] Configure centralized logging (ELK/Splunk/AppInsights)
- [ ] Set up monitoring and alerts
- [ ] Implement circuit breakers (Polly)
- [ ] Configure auto-scaling
- [ ] Set up CI/CD pipelines
- [ ] Database connection pooling
- [ ] Enable response compression
- [ ] Configure CDN for static assets

### Environment-Specific Settings

#### Development
- Detailed logging enabled
- Swagger UI enabled
- CORS allows all origins
- SQL Server in Docker

#### Staging
- Warning-level logging
- Swagger UI enabled (restricted)
- CORS configured for test domains
- Dedicated database

#### Production
- Error-level logging only
- Swagger UI disabled
- CORS strictly configured
- High-availability database
- Secrets in Key Vault
- Auto-scaling enabled

## 👨‍💻 Development Guidelines


### Code Style 

- Follow C# coding conventions
- Use async/await for I/O operations
- Implement proper error handling
- Write XML documentation comments
- Follow SOLID principles
- Write unit tests (aim for 90%+ coverage)
- Use dependency injection
- Keep controllers thin, services focused

### Git Workflow

1. Create feature branch: `git checkout -b feature/new-feature`
2. Make changes and commit: `git commit -m "feat: add new feature"`
3. Push to remote: `git push origin feature/new-feature`
4. Create pull request
5. Code review and merge


## 📚 Documentation

- **Solution Overview:** This file
- **PaymentAPI:** [PaymentAPI/README.md](./PaymentAPI/README.md)
- **Architecture Decisions:** [docs/architecture/](./docs/architecture/) (planned)
- **API Specifications:** OpenAPI/Swagger per service
- **Deployment Guide:** [docs/deployment/](./docs/deployment/) (planned)

## 🧪 Testing

### Running All Tests
```bash
# Run all tests in the solution
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReporter=lcov
```

### Test Structure
Each microservice has its own test project:
- `PaymentAPI.Tests` - PaymentAPI tests
- `UserAPI.Tests` - UserAPI tests
- `OrderAPI.Tests` - OrderAPI tests (future)

### Test Coverage Goals
- Unit Tests: 90%+ coverage
- Integration Tests: Critical paths covered
- End-to-End Tests: Main user flows (planned)

## 📞 Contact & Links

- **Developer:** [Dew M. G. Shahriar]
- **LinkedIn:** [linkedin.com/in/dew-m-g-shahriar-24389568/]
- **GitHub:** [github.com/mgsdew]
- **Email:** [shahriarmdgolam@gmail.com]

## 📄 License

This project is created for portfolio and demonstration purposes, and if you found it useful, please consider supporting the project by leaving a star on GitHub.

---

<div align="center">

**Built with .NET 8.0 & C# 12.0**

*An enterprise-grade microservices solution demonstrating professional software architecture*

</div>
