# UserAPI - User Management Microservice

> **A comprehensive user management microservice with authentication, user CRUD operations, and role-based access control.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Tests](https://img.shields.io/badge/Tests-Passing-success)](https://github.com)

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or use the provided Docker setup)
- Access to parent `.env` file with authentication tokens

### Running the API

```bash
# Navigate to UserAPI directory
cd UserAPI

# Restore dependencies
dotnet restore

# Run the API
dotnet run --project UserAPI/UserAPI.csproj

# API will be available at: http://localhost:5160
# Swagger UI: http://localhost:5160/swagger
```

### Testing the API

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📋 API Endpoints

### User Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/User` | Get all users |
| `GET` | `/api/User/by-id/{guid}` | Get user by GUID |
| `GET` | `/api/User/{username}` | Get user by username |
| `GET` | `/api/User/by-email?email=user@example.com` | Get user by email |
| `POST` | `/api/User` | Create new user |
| `PUT` | `/api/User/{username}` | Update user by username |
| `DELETE` | `/api/User/{username}` | Delete user by username |
| `POST` | `/api/User/{username}/assign-role` | Assign role to user (admin only) |
| `POST` | `/api/User/authenticate` | Authenticate user |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check endpoint |

## 🔐 Authentication

All endpoints (except `/health`) require authentication via `Authorization` header:

```
Authorization: Bearer your-token-here
```

Valid tokens are configured in the parent `.env` file under `AUTH_VALID_TOKENS`.

## 👥 Role Management

### User Roles
Users can have one of the following roles:
- **User**: Standard user with basic access
- **Admin**: Administrative user with elevated privileges
- **Moderator**: User with moderation capabilities

### Role Assignment
- Roles are **nullable** at user creation time (set to `null`)
- Only **Admin** users can assign roles to other users
- Role assignment is done via the `POST /api/User/{username}/assign-role` endpoint
- The requesting admin must be identified via the `X-Requesting-User` header
- Users cannot assign roles to themselves

### Security Features
- Type-safe role management using enums
- Admin-only role assignment prevents privilege escalation
- Proper authorization checks in the service layer
- Comprehensive test coverage for security scenarios

## 📝 API Examples

### Get All Users
```bash
curl -X GET "http://localhost:5160/api/User" \
     -H "Authorization: Bearer dev-token-123456" \
     -H "accept: application/json"
```

### Get User by Username
```bash
curl -X GET "http://localhost:5160/api/User/admin" \
     -H "Authorization: Bearer dev-token-123456" \
     -H "accept: application/json"
```

### Create User
```bash
curl -X POST "http://localhost:5160/api/User" \
     -H "Authorization: Bearer dev-token-123456" \
     -H "Content-Type: application/json" \
     -d '{
       "username": "johndoe",
       "email": "john.doe@example.com",
       "firstName": "John",
       "lastName": "Doe",
       "password": "securepassword123",
       "isActive": true
     }'
```

### Update User by Username
```bash
curl -X PUT "http://localhost:5160/api/User/johndoe" \
     -H "Authorization: Bearer dev-token-123456" \
     -H "Content-Type: application/json" \
     -d '{
       "username": "johndoe",
       "email": "john.doe.updated@example.com",
       "firstName": "John",
       "lastName": "Doe",
       "isActive": true
     }'
```

### Delete User by Username
```bash
curl -X DELETE "http://localhost:5160/api/User/johndoe" \
     -H "Authorization: Bearer dev-token-123456"
```

### Authenticate User
```bash
curl -X POST "http://localhost:5160/api/User/authenticate" \
     -H "Authorization: Bearer dev-token-123456" \
     -H "Content-Type: application/json" \
     -d '{
       "usernameOrEmail": "admin",
       "password": "password123"
     }'
```

### Assign Role to User (Admin Only)
```bash
curl -X POST "http://localhost:5160/api/User/johndoe/assign-role" \
     -H "Authorization: Bearer dev-token-123456" \
     -H "X-Requesting-User: admin" \
     -H "Content-Type: application/json" \
     -d '{
       "role": "Admin"
     }'
```

## 🏗 Architecture

### Clean Architecture Pattern with Database Persistence

```
UserAPI/
├── Controllers/          # HTTP API endpoints
├── Services/            # Business logic layer
│   └── Interfaces/      # Service contracts
├── Repositories/        # Data access layer (EF Core)
│   └── Interfaces/      # Repository contracts
├── Models/              # Domain models
│   ├── DTOs/           # Data transfer objects
│   └── Entities/       # Domain entities (EF Core)
├── Middleware/          # Custom middleware
└── Properties/          # Launch settings
```

### Key Components

- **UserController**: RESTful API endpoints
- **UserService**: Business logic and validation
- **UserRepository**: Data access (EF Core with SQL Server)
- **AuthTokenMiddleware**: Token-based authentication
- **User Entity**: Core user domain model with GUID IDs
- **DTOs**: Request/response data transfer objects

### Database Integration

- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server with configurable connection
- **Migrations:** Code-first approach with automatic migrations
- **Tables:** Users with proper indexing and constraints

## 🧪 Testing

### Test Coverage
- **Unit Tests**: Service and repository layers
- **Integration Tests**: Controller endpoints
- **Test Framework**: xUnit with Moq and FluentAssertions

### Running Tests
```bash
# From UserAPI directory
dotnet test

# With detailed output
dotnet test --verbosity normal

# With coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

## 🔧 Configuration

### Environment Variables
The API loads configuration from the parent `.env` file:
- `AUTH_VALID_TOKENS`: Semicolon-separated list of valid authentication tokens

### Appsettings
- `appsettings.json`: Production configuration
- `appsettings.Development.json`: Development-specific settings

## 🚀 Deployment

### Docker Support
```bash
# Build Docker image
docker build -t userapi .

# Run container
docker run -p 5160:80 userapi
```

### Production Considerations
- Implement proper password hashing (BCrypt/Argon2)
- Add rate limiting and request validation
- Configure proper logging and monitoring
- Set up health checks and metrics
- Enable service discovery registration with Consul

## 📊 Sample Data

The API includes sample users for testing:

1. **Admin User**
   - Username: `admin`
   - Email: `admin@example.com`
   - Role: `Admin`

2. **Regular User**
   - Username: `johndoe`
   - Email: `user@example.com`
   - Role: `User`

## 🔗 Related Services

- **PaymentAPI**: Payment processing microservice
- **API Gateway**: Centralized routing and authentication (planned)
- **Database**: Shared database for all microservices (planned)

---

*Part of the SalesSystem enterprise microservices solution*