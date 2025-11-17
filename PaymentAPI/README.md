# Payment Card Validation API 💳

> **Credit card validation microservice using the Luhn Algorithm with database persistence and token-based authentication.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Tests](https://img.shields.io/badge/Tests-56%20Passed-success)](https://github.com)
[![Coverage](https://img.shields.io/badge/Coverage-95%25-brightgreen)](https://github.com)

## 📋 Quick Links

- [Solution Overview](../README.md) - Architecture, environment setup, and project goals
- [API Endpoints](#-api-endpoints) - Available endpoints and usage
- [Testing Examples](#-testing-examples) - cURL, Swagger, and code examples
- [Supported Cards](#-supported-card-types) - Card types and validation

## 📖 Overview

The **Payment Card Validation API** is a microservice that validates credit card numbers using the **Luhn Algorithm** (modulus 10), the industry-standard checksum formula used by payment processors worldwide. Validated card information is persisted to a database for audit and tracking purposes.

### Key Features

- ✅ **Luhn Algorithm Validation** - Industry-standard credit card validation
- ✅ **Multi-Card Detection** - Visa, MasterCard, American Express, Discover
- ✅ **Database Persistence** - EF Core integration with SQL Server
- ✅ **Token Authentication** - Secure endpoint protection
- ✅ **Card Masking** - PCI-DSS compliant data protection (shows only last 4 digits)
- ✅ **RESTful Design** - Standard HTTP methods and status codes
- ✅ **CRUD Operations** - Create, Read, Delete operations for card validation records
- ✅ **Inter-Service Communication** - HTTP client integration with UserAPI
- ✅ **Event Publishing** - RabbitMQ integration for asynchronous events
- ✅ **Comprehensive Tests** - 56 unit tests with 95%+ coverage
- ✅ **Swagger/OpenAPI** - Interactive API documentation

## 🚀 Quick Start - Running the API

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or use the provided Docker setup)

### Command to Run PaymentAPI:

```bash
# From the SalesSystem root directory
cd PaymentAPI
dotnet restore
dotnet build
dotnet run --project PaymentAPI/PaymentAPI.csproj
```

### Access the API:
- **API Base URL:** `http://localhost:5159`
- **Swagger UI:** `http://localhost:5159/swagger` (Open in browser)
- **Health Check:** `http://localhost:5159/api/CardPayment/health`

## 🔌 API Endpoints

### 1. Validate Credit Card 🔒

**Endpoint:** `POST /api/CardPayment/validate`  
**Authentication:** Required (Bearer Token)

**Description:** Validates a credit card number using the Luhn algorithm and persists the validation result to the database.

**Request:**
```bash
curl -X POST http://localhost:5159/api/CardPayment/validate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-token-here" \
  -d '{"cardNumber": "4532015112830366"}'
```

**Success Response (200 OK):**
```json
{
  "isValid": true,
  "maskedCardNumber": "************0366",
  "cardType": "Visa",
  "message": "Card number is valid",
  "validatedAt": "2025-11-16T10:30:00.000Z"
}
```

### 2. Delete Card Records 🔒

**Endpoint:** `DELETE /api/CardPayment/delete/{cardNumber}`  
**Authentication:** Required (Bearer Token)

**Description:** Deletes all card payment validation records associated with the specified card number.

**Request:**
```bash
curl -X DELETE http://localhost:5159/api/CardPayment/delete/4532015112830366 \
  -H "Authorization: Bearer your-token-here"
```

**Success Response (200 OK):**
```json
{
  "message": "Card payment records deleted successfully",
  "cardNumber": "4532015112830366"
}
```

**Not Found Response (404):**
```json
{
  "status": 404,
  "title": "Not Found",
  "detail": "No card payment records found for card number: 4532015112830366"
}
```

### 3. Health Check ✅

**Endpoint:** `GET /api/CardPayment/health`  
**Authentication:** Not Required

**Description:** Health check endpoint to verify the API is running and database connectivity.

```bash
curl http://localhost:5159/api/CardPayment/health
```

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2025-11-16T10:30:00.000Z"
}
```

## 💳 Supported Card Types

| Card Type | IIN Range | Length | Example |
|-----------|-----------|--------|---------|
| **Visa** | 4 | 13-19 | 4532015112830366 |
| **MasterCard** | 51-55, 2221-2720 | 16 | 5425233430109903 |
| **American Express** | 34, 37 | 15 | 374245455400126 |
| **Discover** | 6011, 622126-622925, 644-649, 65 | 16 | 6011111111111117 |

## 🧪 Testing Examples

### Test 1: Valid Visa Card ✅
```bash
curl -X POST http://localhost:5159/api/CardPayment/validate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-token-here" \
  -d '{"cardNumber": "4532015112830366"}'
```

### Test 2: Valid MasterCard ✅
```bash
curl -X POST http://localhost:5159/api/CardPayment/validate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-token-here" \
  -d '{"cardNumber": "5425233430109903"}'
```

### Test 3: Invalid Card (Wrong Checksum) ❌
```bash
curl -X POST http://localhost:5159/api/CardPayment/validate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer your-token-here" \
  -d '{"cardNumber": "4532015112830367"}'
```

### Test 4: Delete Card Records 🗑️
```bash
curl -X DELETE http://localhost:5159/api/CardPayment/delete/4532015112830366 \
  -H "Authorization: Bearer your-token-here"
```

### Test 5: No Authentication (Should fail) 🔒
```bash
curl -i -X POST http://localhost:5159/api/CardPayment/validate \
  -H "Content-Type: application/json" \
  -d '{"cardNumber": "4532015112830366"}'
```

**Expected: 401 Unauthorized**

## 🔐 Authentication

All endpoints (except `/health` and `/swagger`) require authentication via Bearer token.

**Usage:**
```http
Authorization: Bearer your-token-here
```

## 🧮 Luhn Algorithm

The Luhn algorithm validates credit card numbers using a checksum formula:

1. **Start from the rightmost digit** (check digit)
2. **Double every second digit** moving left
3. **If doubled value > 9**, subtract 9
4. **Sum all digits**
5. **If sum % 10 = 0**, the number is valid

**Example: 4532015112830366**
```
Original: 4 5 3 2 0 1 5 1 1 2 8 3 0 3 6 6
Doubled:  8 5 6 2 0 1 1 1 2 2 7 3 0 3 3 6
Sum: 8+5+6+2+0+1+1+1+2+2+7+3+0+3+3+6 = 50
50 % 10 = 0 ✅ Valid!
```

## 🗄️ Database Integration

The API uses Entity Framework Core for database operations:

- **Table:** `tblCardPayment`
- **Fields:** CardNumber, CardType, IsValid, ValidatedAt, CreatedAt
- **CardType:** Enum-based (Visa, MasterCard, Amex, Discover)
- **Connection:** Configurable via appsettings.json

## 🔗 Inter-Service Communication

The PaymentAPI integrates with other microservices through multiple communication patterns:

### HTTP Client Integration
- **Typed HTTP Client** for synchronous communication with UserAPI
- **User Validation** during payment processing
- **Dependency Injection** for loose coupling and testability

### Message Queue Integration
- **RabbitMQ Publisher** for asynchronous event publishing
- **Topic Exchange** for event-driven architecture
- **Payment Validation Events** published to `salessystem.events` exchange

### Service Architecture
```
PaymentAPI
├── Controllers/               # REST endpoints
├── Services/                  # Business logic + communication
│   ├── CardPaymentService.cs  # Core payment logic
│   ├── UserApiClient.cs       # HTTP client for UserAPI
│   ├── MessagePublisher.cs    # RabbitMQ event publisher
│   └── Interfaces/            # Communication contracts
├── Repositories/              # Data access (EF Core)
└── Models/                    # DTOs and Entities
```

## 🧪 Running Unit Tests

```bash
# From PaymentAPI directory
cd PaymentAPI
dotnet test

# With detailed output
dotnet test --logger "console;verbosity=detailed"
```

**Test Results:**
- Total tests: 56
- All passed: ✅
- Coverage: 95%+

## 📁 Project Structure

```
PaymentAPI/
├── PaymentAPI/                    # Main API Project
│   ├── Controllers/               # REST endpoints
│   │   └── CardPaymentController.cs
│   ├── Services/                  # Business logic + communication
│   │   ├── CardPaymentService.cs  # Core payment logic
│   │   ├── UserApiClient.cs       # HTTP client for UserAPI
│   │   ├── MessagePublisher.cs    # RabbitMQ event publisher
│   │   └── Interfaces/            # Communication contracts
│   │       ├── IUserApiClient.cs
│   │       └── IMessagePublisher.cs
│   ├── Repositories/              # Data access (EF Core)
│   │   ├── CardPaymentRepository.cs
│   │   └── Interfaces/
│   ├── Models/                    # DTOs and Entities
│   │   ├── DTOs/
│   │   │   ├── CardPaymentRequestDto.cs
│   │   │   └── CardPaymentResponseDto.cs
│   │   └── Entities/
│   │       └── CardPayment.cs
│   ├── Middleware/                # Authentication
│   │   └── AuthTokenMiddleware.cs
│   ├── appsettings.json           # Configuration
│   ├── Program.cs                 # Application entry point
│   └── PaymentAPI.csproj          # Project file
├── PaymentAPI.Tests/              # Unit tests (56 tests)
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   └── GlobalUsings.cs
├── PaymentAPI.sln                 # Solution file
└── README.md                      # This file
```

## 📞 Contact

- **Email:** shahriarmdgolam@gmail.com
- **LinkedIn:** [dew-m-g-shahriar](https://www.linkedin.com/in/dew-m-g-shahriar-24389568/)
- **GitHub:** [SalesSystem](https://github.com/mgsdew/SalesSystem)

---

<div align="center">

**Part of the SalesSystem Microservices Solution**

[Back to Solution Overview](../README.md)

</div>
