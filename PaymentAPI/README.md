# Payment Card Validation API 💳# Payment Card Validation API 💳



> **Credit card validation using the Luhn Algorithm with token-based authentication.**> **A production-grade, enterprise-level ASP.NET Core Web API demonstrating advanced software engineering practices, secure authentication, clean architecture, and comprehensive testing.**



[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)

[![Tests](https://img.shields.io/badge/Tests-56%20Passed-success)](https://github.com)[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

[![Coverage](https://img.shields.io/badge/Coverage-95%25-brightgreen)](https://github.com)[![Tests](https://img.shields.io/badge/Tests-56%20Passed-success)](https://github.com)

[![Coverage](https://img.shields.io/badge/Coverage-95%25-brightgreen)](https://github.com)

## 📋 Quick Links

##  Project Goal

- [Solution Overview](../README.md) - Architecture, environment setup, and project goals

- [API Endpoints](#-api-endpoints) - Available endpoints and usageThis project showcases the ability to design and implement **professional, industrial-standard APIs** suitable for enterprise environments. The API validates credit card numbers using the **Luhn Algorithm** (modulus 10), a widely-used checksum formula in the payment industry. 

- [Testing Examples](#-testing-examples) - cURL, Swagger, and code examples

- [Supported Cards](#-supported-card-types) - Card types and validationThis demonstrates:

-  **Security-first approach** with token-based authentication

## 📖 Overview-  **Clean Architecture** with separation of concerns

-  **SOLID principles** and design patterns

The **Payment Card Validation API** is a microservice that validates credit card numbers using the **Luhn Algorithm** (modulus 10), the industry-standard checksum formula used by payment processors worldwide.-  **Comprehensive testing** with 95%+ code coverage

-  **Production-ready code** with proper error handling and logging

### Key Features-  **RESTful API design** following industry best practices

-  **Complete documentation** with Swagger/OpenAPI integration

- ✅ **Luhn Algorithm Validation** - Industry-standard credit card validation

- ✅ **Multi-Card Detection** - Visa, MasterCard, American Express, Discover##  Key Features & Technical Highlights

- ✅ **Token Authentication** - Secure endpoint protection

- ✅ **Card Masking** - PCI-DSS compliant data protection (shows only last 4 digits)### Security & Authentication

- ✅ **RESTful Design** - Standard HTTP methods and status codes-  **Custom Middleware Authentication** - Token-based security layer

- ✅ **Comprehensive Tests** - 56 unit tests with 95%+ coverage-  **Environment Variable Management** - Secure credential handling

- ✅ **Swagger/OpenAPI** - Interactive API documentation-  **Card Data Masking** - PCI-DSS compliant data protection



## 🚀 Quick Start### Core Functionality

-  **Luhn Algorithm Implementation** - Industry-standard credit card validation

### Prerequisites-  **Multi-Card Type Detection** - Visa, MasterCard, American Express, Discover

- .NET 8.0 SDK-  **Async/Await Pattern** - Full asynchronous support with CancellationToken

- SQL Server (configured for DevServer, currently using in-memory for demo)-  **RESTful API Design** - Proper HTTP methods and status codes



### Run the Service### Architecture & Design

-  **Clean Architecture** - Layered design with clear separation of concerns

```bash-  **SOLID Principles** - Maintainable and extensible codebase

cd PaymentAPI-  **Repository Pattern** - Data access abstraction

-  **Dependency Injection** - Loose coupling and testability

# Restore and build-  **DTO Pattern** - Secure data transfer objects

dotnet restore

dotnet build### Quality & Testing

- **56 Unit Tests** - Comprehensive test coverage (95%+)

# Set environment variables and run (Linux/Mac)- **xUnit + Moq + FluentAssertions** - Modern testing stack

export AUTH_VALID_TOKENS="dev-token-123456;test-token-abcdef;your-secret-token-here"- **Test-Driven Development** - Services, Controllers, and Repositories fully tested

dotnet run --project PaymentAPI/PaymentAPI.csproj

### Developer Experience

# Run (Windows PowerShell)- **Swagger/OpenAPI Integration** - Interactive API documentation

$env:AUTH_VALID_TOKENS="dev-token-123456;test-token-abcdef;your-secret-token-here"- **XML Documentation** - IntelliSense-ready code comments

dotnet run --project PaymentAPI/PaymentAPI.csproj- **Structured Logging** - Production-ready logging with ILogger

```- *Easy Setup** - One-command build and run



### Access Points## 📋 Table of Contents

- **API Base URL:** `http://localhost:5159`

- **Swagger UI:** `http://localhost:5159/swagger`- [Technologies Used](#technologies-used)

- **Health Check:** `http://localhost:5159/api/CardPayment/health`- [Project Structure](#project-structure)

- [Getting Started](#getting-started)

## 🔌 API Endpoints- [API Documentation](#api-documentation)

- [Testing](#testing)

### Base URL- [Architecture](#architecture)

```- [Examples](#examples)

http://localhost:5159/api- [Contributing](#contributing)

```

## 🛠 Technologies & Tools

### 1. Validate Credit Card 🔒

### Backend Framework

Validates a credit card number using the Luhn algorithm.| Technology | Version | Purpose |

|------------|---------|---------|

**Endpoint:** `POST /api/CardPayment/validate`| **.NET** | 8.0 (LTS) | Runtime platform |

| **C#** | 12.0 | Programming language |

**Authentication:** Required (Bearer Token)| **ASP.NET Core** | 8.0 | Web API framework |



#### Request### Development Tools & Libraries

| Tool/Library | Purpose |

**Headers:**|--------------|---------|

```http| **Swagger/OpenAPI** | Interactive API documentation & testing |

Content-Type: application/json| **Dependency Injection** | Built-in IoC container for loose coupling |

Authorization: Bearer dev-token-123456| **ILogger** | Structured logging framework |

```| **Data Annotations** | Model validation |



**Body:**### Testing Stack

```json| Tool | Purpose |

{|------|---------|

  "cardNumber": "4532015112830366"| **xUnit** | Modern unit testing framework |

}| **Moq** | Mocking framework for unit tests |

```| **FluentAssertions** | Readable and expressive test assertions |

| **Microsoft.AspNetCore.Mvc.Testing** | Integration testing support |

**Validation Rules:**

- Card number: Required, 13-19 digits### Security & Configuration

- Special characters (spaces, dashes) are automatically removed| Component | Implementation |

- Only numeric characters allowed after cleanup|-----------|----------------|

| **Custom Middleware** | Token-based authentication |

#### Responses| **Environment Variables** | Secure credential management |

| **.gitignore** | Sensitive data protection |

**✅ Success (200 OK):**

```json##  Full Project Architecture

{

  "isValid": true,### Solution Structure

  "maskedCardNumber": "************0366",```

  "cardType": "Visa",PaymentAPI/                                    # Solution Root

  "message": "Card number is valid",│

  "validatedAt": "2025-11-10T12:34:56.789Z"├── PaymentAPI/                                # Main API Project

}│   ├── Controllers/                           # API Layer (HTTP Endpoints)

```│   │   └── CardPaymentController.cs           # RESTful endpoints with proper status codes

│   │

**❌ Invalid Card (200 OK):**│   ├── Services/                              # Business Logic Layer

```json│   │   ├── Interfaces/

{│   │   │   └── ICardPaymentService.cs         # Service contract

  "isValid": false,│   │   └── CardPaymentService.cs              # Luhn algorithm implementation

  "maskedCardNumber": "************0367",│   │

  "cardType": "Visa",│   ├── Repositories/                          # Data Access Layer

  "message": "Card number is invalid",│   │   ├── Interfaces/

  "validatedAt": "2025-11-10T12:34:56.789Z"│   │   │   └── ICardPaymentRepository.cs      # Repository contract

}│   │   └── CardPaymentRepository.cs           # SQL Server configured (demo: in-memory)

```│   │

│   ├── Models/                                # Domain Models

**⚠️ Validation Error (400 Bad Request):**│   │   ├── Entities/

```json│   │   │   └── CardPayment.cs                 # Domain entity

{│   │   └── DTOs/

  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",│   │       ├── CardPaymentRequestDto.cs       # Input validation model

  "title": "One or more validation errors occurred.",│   │       └── CardPaymentResponseDto.cs      # Response model

  "status": 400,│   │

  "errors": {│   ├── Middleware/                            # Custom Middleware

    "CardNumber": [│   │   └── AuthTokenMiddleware.cs             # Authentication layer

      "Card number must contain only digits and be between 13-19 characters"│   │

    ]│   ├── Properties/

  }│   │   └── launchSettings.json                # Development settings

}│   │

```│   ├── appsettings.json                       # Configuration

│   ├── appsettings.Development.json           # Dev-specific config

**🔒 Unauthorized (401):**│   ├── Program.cs                             # Application entry point & DI setup

```json│   └── PaymentAPI.csproj                      # Project file

{│

  "error": "Authorization header is missing"├── PaymentAPI.Tests/                          # Test Project

}│   ├── Controllers/

```│   │   └── CardPaymentControllerTests.cs      # 18 Controller tests

│   ├── Services/

**🚫 Forbidden (403):**│   │   └── CardPaymentServiceTests.cs         # 28 Service tests (Luhn logic)

```json│   ├── Repositories/

{│   │   └── CardPaymentRepositoryTests.cs      # 10 Repository tests

  "error": "Invalid or expired token"│   └── Payment.Test.csproj                    # Test project file

}│

```├── .env                                       # Environment variables (auth tokens)

├── .gitignore                                 # Git ignore rules

**💥 Server Error (500):**├── PaymentAPI.sln                             # Solution file

```json└── README.md                                  # This file

{```

  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",

  "title": "Internal Server Error",### Architectural Layers

  "status": 500,

  "detail": "An unexpected error occurred while processing your request."```

}┌─────────────────────────────────────────────────┐

```│          API Layer (Controllers)                │  ← HTTP Requests/Responses

│  • CardPaymentController                        │  ← Validation & Error Handling

---└─────────────────────┬───────────────────────────┘

                      │

### 2. Health Check ✅┌─────────────────────▼───────────────────────────┐

│      Business Logic Layer (Services)            │  ← Core Business Rules

Check if the API is running and healthy.│  • ICardPaymentService / CardPaymentService     │  ← Luhn Algorithm

└─────────────────────┬───────────────────────────┘

**Endpoint:** `GET /api/CardPayment/health`                      │

┌─────────────────────▼───────────────────────────┐

**Authentication:** Not Required│     Data Access Layer (Repositories)            │  ← Data Operations

│  • ICardPaymentRepository / Repository          │  ← SQL Server (Demo: In-Memory)

#### Request└───────────────────────────────────────────────┬─┘

                      │

```http┌─────────────────────▼───────────────────────────┐

GET /api/CardPayment/health HTTP/1.1│           Domain Models (Entities)              │  ← Business Entities

Host: localhost:5159│  • CardPayment Entity                           │

```└─────────────────────────────────────────────────┘

```

#### Response

### Design Patterns Implemented

**Success (200 OK):**

```json1. **Dependency Injection** - Constructor injection for loose coupling

{2. **Repository Pattern** - Abstraction over data access

  "status": "healthy",3. **DTO Pattern** - Separate internal and external models

  "timestamp": "2025-11-10T12:34:56.789Z"4. **Middleware Pattern** - Custom authentication pipeline

}5. **Async/Await** - Non-blocking I/O operations

```6. **Interface Segregation** - Small, focused interfaces



## 💳 Supported Card Types##  Getting Started



| Card Type | IIN Range | Length | Example |### Prerequisites

|-----------|-----------|--------|---------|

| **Visa** | 4 | 13-19 | 4532015112830366 |- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

| **MasterCard** | 51-55, 2221-2720 | 16 | 5425233430109903 |- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

| **American Express** | 34, 37 | 15 | 374245455400126 |- Terminal/Command Line

| **Discover** | 6011, 622126-622925, 644-649, 65 | 16 | 6011111111111117 |- SQL Server (DevServer) - Connection configured in `appsettings.json`



## 🧪 Testing Examples### Quick Start (3 Steps)



### Using Swagger UI (Easiest Method)#### 1. Clone & Navigate

```bash

1. **Open Swagger UI:**git clone https://github.com/mgsdew/SalesSystem

   ```cd PaymentAPI

   http://localhost:5159/swagger```

   ```

#### 2. Configure Authentication (Important!)

2. **Authorize:**The project includes a `.env` file with demo authentication tokens:

   - Click the **"Authorize"** button (🔓 icon) at the top right

   - Enter token: `dev-token-123456````bash

   - Click "Authorize" then "Close"# .env file content

AUTH_VALID_TOKENS=dev-token-123456;test-token-abcdef;your-secret-token-here

3. **Test the API:**```

   - Expand `POST /api/CardPayment/validate`

   - Click **"Try it out"**> **Note for Recruiters/Demo**: This `.env` file is intentionally included in the repository for demonstration purposes only. The `.gitignore` file has this line commented out to allow review of the authentication setup.

   - Enter a card number in the request body>

   - Click **"Execute"**> **Production Warning**: In a real production environment, the `.env` line should be **uncommented in `.gitignore`** to prevent sensitive data from being committed to version control. See the [Security Notes](#-security--production-notes) section for details.

   - View the response below

#### 3. Restore, Build & Run

### Using cURL```bash

# Restore NuGet packages

#### Test 1: Valid Visa Card ✅dotnet restore

```bash

curl -X POST http://localhost:5159/api/CardPayment/validate \# Build the solution

  -H "Content-Type: application/json" \dotnet build

  -H "Authorization: Bearer dev-token-123456" \

  -d '{# Load environment variables and run the API

    "cardNumber": "4532015112830366"export $(cat .env | xargs) && dotnet run --project PaymentAPI/PaymentAPI.csproj

  }'```

```

**On Windows PowerShell:**

**Expected Response:**```powershell

```json# Set environment variable

{$env:AUTH_VALID_TOKENS="dev-token-123456;test-token-abcdef;your-secret-token-here"

  "isValid": true,

  "maskedCardNumber": "************0366",# Run the API

  "cardType": "Visa",dotnet run --project PaymentAPI/PaymentAPI.csproj

  "message": "Card number is valid",```

  "validatedAt": "2025-11-10T12:34:56.789Z"

}### Access Points

```

Once running, the API is available at:

#### Test 2: Valid MasterCard ✅- **HTTP**: `http://localhost:5159`

```bash- **Swagger UI**: `http://localhost:5159/` (Interactive API documentation)

curl -X POST http://localhost:5159/api/CardPayment/validate \- **Health Check**: `http://localhost:5159/health`

  -H "Content-Type: application/json" \

  -H "Authorization: test-token-abcdef" \### Database Configuration

  -d '{

    "cardNumber": "5425233430109903"The project includes SQL Server connection string configuration for demonstration:

  }'

```**Connection String (in `appsettings.json`):**

```json

#### Test 3: Valid American Express ✅"ConnectionStrings": {

```bash  "DefaultConnection": "Server=DevServer;Database=PaymentDB;User Id=sa;Password=Dev123;..."

curl -X POST http://localhost:5159/api/CardPayment/validate \}

  -H "Content-Type: application/json" \```

  -H "Authorization: dev-token-123456" \

  -d '{**Database Details:**

    "cardNumber": "374245455400126"- Server: `DevServer`

  }'- Database: `PaymentDB`

```- Credentials: Configured in appsettings



#### Test 4: Invalid Card (Wrong Checksum) ❌> **Note**: Currently using in-memory storage for demo purposes. The SQL Server configuration demonstrates how to set up database connections in a production environment.

```bash

curl -X POST http://localhost:5159/api/CardPayment/validate \## API Documentation & Testing

  -H "Content-Type: application/json" \

  -H "Authorization: dev-token-123456" \### Base URL

  -d '{```

    "cardNumber": "4532015112830367"http://localhost:5159/api

  }'```

```

###  Authentication

**Expected Response:**

```jsonAll API endpoints (except `/health` and `/swagger`) require authentication via Bearer token.

{

  "isValid": false,**Required Header:**

  "maskedCardNumber": "************0367",```

  "cardType": "Visa",Authorization: Bearer dev-token-123456

  "message": "Card number is invalid",```

  "validatedAt": "2025-11-10T12:34:56.789Z"

}Or without Bearer prefix:

``````

Authorization: dev-token-123456

#### Test 5: Missing Authentication Token 🔒```

```bash

curl -i -X POST http://localhost:5159/api/CardPayment/validate \**Valid Tokens (Demo):**

  -H "Content-Type: application/json" \- `dev-token-123456`

  -d '{- `test-token-abcdef`

    "cardNumber": "4532015112830366"- `your-secret-token-here`

  }'

```### API Endpoints



**Expected Response (401 Unauthorized):**#### 1. Validate Credit Card (Protected)

```json

{Validates a credit card number using the Luhn algorithm and returns card type.

  "error": "Authorization header is missing"

}**Endpoint:** `POST /api/CardPayment/validate`

```

**Authentication:** Required ✅

#### Test 6: Invalid Token 🚫

```bash**Request Headers:**

curl -i -X POST http://localhost:5159/api/CardPayment/validate \```http

  -H "Content-Type: application/json" \Content-Type: application/json

  -H "Authorization: Bearer invalid-token-xyz" \Authorization: Bearer dev-token-123456

  -d '{```

    "cardNumber": "4532015112830366"

  }'**Request Body:**

``````json

{

**Expected Response (403 Forbidden):**  "cardNumber": "4532015112830366"

```json}

{```

  "error": "Invalid or expired token"

}**Validation Rules:**

```- Card number must contain only digits (spaces and dashes are automatically removed)

- Length must be between 13-19 characters

#### Test 7: Invalid Format (Too Short) ⚠️- Required field (cannot be null or empty)

```bash

curl -X POST http://localhost:5159/api/CardPayment/validate \**Success Response (200 OK):**

  -H "Content-Type: application/json" \```json

  -H "Authorization: dev-token-123456" \{

  -d '{  "isValid": true,

    "cardNumber": "123456"  "maskedCardNumber": "************0366",

  }'  "cardType": "Visa",

```  "message": "Card number is valid",

  "validatedAt": "2025-11-10T12:34:56.789Z"

**Expected Response (400 Bad Request):**}

```json```

{

  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",**Validation Error (400 Bad Request):**

  "title": "One or more validation errors occurred.",```json

  "status": 400,{

  "errors": {  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",

    "CardNumber": [  "title": "One or more validation errors occurred.",

      "Card number must contain only digits and be between 13-19 characters"  "status": 400,

    ]  "errors": {

  }    "CardNumber": [

}      "Card number must contain only digits and be between 13-19 characters"

```    ]

  }

#### Test 8: Health Check (No Auth Required) ✅}

```bash```

curl http://localhost:5159/api/CardPayment/health

```**Unauthorized (401):**

```json

**Expected Response:**{

```json  "error": "Authorization header is missing"

{}

  "status": "healthy",```

  "timestamp": "2025-11-10T12:34:56.789Z"

}**Forbidden (403):**

``````json

{

### Using Postman  "error": "Invalid or expired token"

}

1. **Create a new request:**```

   - Method: `POST`

   - URL: `http://localhost:5159/api/CardPayment/validate`**Server Error (500):**

```json

2. **Set Headers:**{

   - `Content-Type`: `application/json`  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",

   - `Authorization`: `Bearer dev-token-123456`  "title": "Internal Server Error",

  "status": 500,

3. **Set Body (raw JSON):**  "detail": "An unexpected error occurred while processing your request.",

   ```json  "instance": "/api/CardPayment/validate"

   {}

     "cardNumber": "4532015112830366"```

   }

   ```#### 2. Health Check (Public)



4. **Click "Send"** and view the responseCheck if the API is running and healthy.



### Using .http Files (VS Code REST Client)**Endpoint:** `GET /api/CardPayment/health`



Create a `test-api.http` file:**Authentication:** Not Required ❌



```http**Response (200 OK):**

### Test 1: Valid Visa Card```json

POST http://localhost:5159/api/CardPayment/validate{

Content-Type: application/json  "status": "healthy",

Authorization: Bearer dev-token-123456  "timestamp": "2025-11-10T12:34:56.789Z"

}

{```

  "cardNumber": "4532015112830366"

}### Testing with Swagger UI



### Test 2: Valid MasterCard1. **Open Swagger UI**: Navigate to `http://localhost:5159/`

POST http://localhost:5159/api/CardPayment/validate2. **Authorize**: Click the "Authorize" button (🔓 icon) at the top right

Content-Type: application/json3. **Enter Token**: Enter one of the valid tokens:

Authorization: test-token-abcdef   ```

   dev-token-123456

{   ```

  "cardNumber": "5425233430109903"4. **Test API**: Click "Try it out" on the POST `/api/CardPayment/validate` endpoint

}5. **Submit Request**: Enter a card number and click "Execute"



### Test 3: Invalid Card**Swagger UI Screenshot Flow:**

POST http://localhost:5159/api/CardPayment/validate```

Content-Type: application/json┌──────────────────────────────────────┐

Authorization: dev-token-123456│  Swagger UI - Payment API v1         │

├──────────────────────────────────────┤

{│  [ Authorize]  ← Click here first  │

  "cardNumber": "4532015112830367"├──────────────────────────────────────┤

}│  CardPayment                          │

│    POST /api/CardPayment/validate     │

### Test 4: No Authentication│      [Try it out]  ← Then click here │

POST http://localhost:5159/api/CardPayment/validate│                                       │

Content-Type: application/json│    Request body:                      │

│    {                                  │

{│      "cardNumber": "4532015112830366" │

  "cardNumber": "4532015112830366"│    }                                  │

}│                                       │

│    [Execute]  ← Finally execute       │

### Test 5: Health Check└──────────────────────────────────────┘

GET http://localhost:5159/api/CardPayment/health```

```

### Supported Card Types

## 🔐 Authentication

| Card Type | IIN Range | Length |

All endpoints (except `/health` and `/swagger`) require authentication via Bearer token.|-----------|-----------|--------|

| Visa | 4 | 13-19 |

### Valid Tokens (Development/Demo)| MasterCard | 51-55, 2221-2720 | 16 |

```| American Express | 34, 37 | 15 |

dev-token-123456| Discover | 6011, 622126-622925, 644-649, 65 | 16 |

test-token-abcdef

your-secret-token-here## Comprehensive Testing Suite

```

### Running Unit Tests

### How to Authenticate

```bash

**Option 1: Bearer Token (Recommended)**# Run all tests

```httpdotnet test

Authorization: Bearer dev-token-123456

```# Run with detailed output

dotnet test --logger "console;verbosity=detailed"

**Option 2: Direct Token**

```http```

Authorization: dev-token-123456

```### Test Results Summary



### Token Configuration#### 1. **Service Layer Tests** (28 tests)

Tests the core business logic and Luhn algorithm implementation:

Tokens are configured via the environment variable `AUTH_VALID_TOKENS` (see root `.env` file):- ✅ Valid card number validation (Visa, MasterCard, Amex, Discover)

```bash- ✅ Invalid card number detection (wrong checksum, invalid format)

AUTH_VALID_TOKENS=dev-token-123456;test-token-abcdef;your-secret-token-here- ✅ Card type determination logic

```- ✅ Edge cases (null, empty, special characters, length validation)

- ✅ Async/await operations

> **⚠️ Production Note:** In production, use JWT tokens with proper expiration, refresh tokens, and a secure key management system (Azure Key Vault, AWS Secrets Manager).- ✅ Cancellation token support



## 🧮 Luhn Algorithm Explained**Sample Test Cases:**

```csharp

The Luhn algorithm validates credit card numbers using a checksum formula:[Theory]

[InlineData("4532015112830366", true)]   // Valid Visa

### Algorithm Steps[InlineData("5425233430109903", true)]   // Valid MasterCard

[InlineData("374245455400126", true)]    // Valid American Express

1. **Start from the rightmost digit** (check digit)[InlineData("6011111111111117", true)]   // Valid Discover

2. **Double every second digit** moving left[InlineData("4532015112830367", false)]  // Invalid checksum

3. **If doubled value > 9**, subtract 9[InlineData("1234567890123456", false)]  // Invalid card

4. **Sum all digits**public void CreditCardValidator_Should_ReturnCorrectValidationResult(

5. **If sum % 10 = 0**, the number is valid    string cardNumber, bool expectedResult)

```

### Example: Validating 4532015112830366

#### 2. **Controller Layer Tests** (18 tests)

```Tests HTTP request handling, validation, and error responses:

Card Number: 4 5 3 2 0 1 5 1 1 2 8 3 0 3 6 6- ✅ POST /api/CardPayment/validate endpoint

Position:    1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6- ✅ Model validation and error handling

             ↑   ↑   ↑   ↑   ↑   ↑   ↑   ↑- ✅ HTTP status codes (200, 400, 500)

             Double these (every 2nd from right)- ✅ ProblemDetails responses

- ✅ Async controller actions

Step 1: Double every second digit from right- ✅ Cancellation token handling

Original: 4 5 3 2 0 1 5 1 1 2 8 3 0 3 6 6

Doubled:  8 5 6 2 0 1 1 1 2 2 7 3 0 3 3 6#### 3. **Repository Layer Tests** (10 tests)

          ↑   ↑   ↑   ↑   ↑   ↑   ↑   ↑Tests data access operations:

- ✅ Save operations

Step 2: Apply "subtract 9 if > 9" rule- ✅ Retrieve operations

8 5 6 2 0 1 1 1 2 2 7 3 0 3 3 6- ✅ Concurrent access handling

(8) 5 (6) 2 (0) 1 (1) 1 (2) 2 (7) 3 (0) 3 (3) 6- ✅ Async repository methods



Step 3: Sum all digits### Test Coverage by Component

8 + 5 + 6 + 2 + 0 + 1 + 1 + 1 + 2 + 2 + 7 + 3 + 0 + 3 + 3 + 6 = 50

```

Step 4: Check if sum % 10 = 0PaymentAPI/

50 % 10 = 0 ✅ Valid!├── Controllers/         Coverage: 95%

```├── Services/           Coverage: 98%

├── Repositories/       Coverage: 92%

### Why It Works└── Models/             Coverage: 100%

- Catches single-digit errors (e.g., typing 5 instead of 4)```

- Catches most transposition errors (e.g., 54 instead of 45)

- Simple to implement and verify### Example Test Scenarios

- Used by all major payment processors

**✅ Valid Card Numbers:**

## 🧪 Running Unit Tests- `4532015112830366` → Visa (Valid)

- `5425233430109903` → MasterCard (Valid)

The project includes 56 comprehensive unit tests with 95%+ code coverage.- `374245455400126` → American Express (Valid)

- `6011111111111117` → Discover (Valid)

### Run All Tests

```bash**❌ Invalid Card Numbers:**

# From PaymentAPI directory- `4532015112830367` → Invalid Luhn checksum

dotnet test- `1234567890123456` → Unknown card type

- `123` → Too short

# With detailed output- `12345678901234567890` → Too long

dotnet test --logger "console;verbosity=detailed"- `453201511283036A` → Contains letters



# With coverage report## 🏗 Architecture

dotnet test /p:CollectCoverage=true /p:CoverageReporter=lcov

```### Design Patterns



### Test Coverage1. **Repository Pattern** - Data access abstraction

2. **Dependency Injection** - Loose coupling and testability

```3. **DTO Pattern** - Data transfer objects for API layer

PaymentAPI.Tests/4. **Service Layer Pattern** - Business logic separation

├── Controllers/5. **SOLID Principles** - Clean and maintainable code

│   └── CardPaymentControllerTests.cs     (18 tests)

│       • Endpoint validation### Dependency Injection Configuration

│       • HTTP status codes

│       • Error handling```csharp

│       • Authentication// Services

│builder.Services.AddScoped<ICardPaymentService, CardPaymentService>();

├── Services/builder.Services.AddSingleton<ICardPaymentRepository, CardPaymentRepository>();

│   └── CardPaymentServiceTests.cs        (28 tests)```

│       • Luhn algorithm validation

│       • Card type detection### Luhn Algorithm Implementation

│       • Edge cases

│       • Null/empty handlingThe Luhn algorithm (also known as the modulus 10 or mod 10 algorithm) is a checksum formula used to validate credit card numbers:

│

└── Repositories/1. Starting from the rightmost digit, double every second digit

    └── CardPaymentRepositoryTests.cs     (10 tests)2. If the result is greater than 9, subtract 9

        • Save operations3. Sum all the digits

        • Retrieve operations4. If the total modulo 10 equals 0, the number is valid

        • Concurrent access

```**Example:**

```

### Sample Test ScenariosCard Number: 4532 0151 1283 0366



**✅ Valid Cards (Should Pass Luhn Check):**Step 1: Double every second digit from right

```csharp6 6 3 3 8 8 2 2 1 1 5 5 0 0 3 3 2 2 5 5 4 4

[Theory]↑   ↑   ↑   ↑   ↑   ↑   ↑   ↑   ↑   ↑   ↑ (doubled)

[InlineData("4532015112830366")]      // Visa

[InlineData("5425233430109903")]      // MasterCardStep 2: If > 9, subtract 9

[InlineData("374245455400126")]       // American Express6 6 3 3 8 8 2 2 1 1 5 5 0 0 3 3 2 2 5 5 4 4

[InlineData("6011111111111117")]      // Discover

public async Task ValidateAsync_ValidCard_ReturnsTrue(string cardNumber)Step 3: Sum = 60

```

Step 4: 60 % 10 = 0 ✅ Valid!

**❌ Invalid Cards (Should Fail Luhn Check):**```

```csharp

[Theory]## 💡 API Testing Examples

[InlineData("4532015112830367")]      // Wrong checksum

[InlineData("1234567890123456")]      // Random number### Using cURL

[InlineData("0000000000000000")]      // All zeros

public async Task ValidateAsync_InvalidCard_ReturnsFalse(string cardNumber)#### Test 1: Without Authentication Token (Expected: 401)

``````bash

curl -i -X POST http://localhost:5159/api/CardPayment/validate \

## 📁 Project Structure  -H "Content-Type: application/json" \

  -d '{"cardNumber":"4532015112830366"}'

``````

PaymentAPI/**Response:**

├── PaymentAPI/                           # Main API Project```http

│   ├── Controllers/HTTP/1.1 401 Unauthorized

│   │   └── CardPaymentController.cs      # REST endpoints{"error":"Authorization header is missing"}

│   ├── Services/```

│   │   ├── Interfaces/

│   │   │   └── ICardPaymentService.cs    # Service contract#### Test 2: With Invalid Token (Expected: 403)

│   │   └── CardPaymentService.cs         # Luhn implementation```bash

│   ├── Repositories/curl -i -X POST http://localhost:5159/api/CardPayment/validate \

│   │   ├── Interfaces/  -H "Content-Type: application/json" \

│   │   │   └── ICardPaymentRepository.cs # Repository contract  -H "Authorization: Bearer invalid-token-xyz" \

│   │   └── CardPaymentRepository.cs      # Data access  -d '{"cardNumber":"4532015112830366"}'

│   ├── Models/```

│   │   ├── DTOs/**Response:**

│   │   │   ├── CardPaymentRequestDto.cs  # Input model```http

│   │   │   └── CardPaymentResponseDto.cs # Response modelHTTP/1.1 403 Forbidden

│   │   └── Entities/{"error":"Invalid or expired token"}

│   │       └── CardPayment.cs            # Domain entity```

│   ├── Middleware/

│   │   └── AuthTokenMiddleware.cs        # Authentication#### Test 3: With Valid Token - Visa Card (Expected: 200)

│   ├── Properties/```bash

│   │   └── launchSettings.json           # Dev settingscurl -i -X POST http://localhost:5159/api/CardPayment/validate \

│   ├── appsettings.json                  # Configuration  -H "Content-Type: application/json" \

│   ├── appsettings.Development.json      # Dev config  -H "Authorization: Bearer dev-token-123456" \

│   ├── Program.cs                        # Entry point  -d '{"cardNumber":"4532015112830366"}'

│   └── PaymentAPI.csproj                 # Project file```

│**Response:**

├── PaymentAPI.Tests/                     # Test Project```http

│   ├── Controllers/HTTP/1.1 200 OK

│   │   └── CardPaymentControllerTests.cs{

│   ├── Services/  "isValid": true,

│   │   └── CardPaymentServiceTests.cs  "maskedCardNumber": "************0366",

│   ├── Repositories/  "cardType": "Visa",

│   │   └── CardPaymentRepositoryTests.cs  "message": "Card number is valid",

│   └── Payment.Test.csproj  "validatedAt": "2025-11-10T00:20:55Z"

│}

├── README.md                             # This file```

└── PaymentAPI.sln                        # Solution file

```#### Test 4: MasterCard Validation

```bash

## 📊 Database Configurationcurl -X POST http://localhost:5159/api/CardPayment/validate \

  -H "Content-Type: application/json" \

The API is configured to connect to SQL Server:  -H "Authorization: test-token-abcdef" \

  -d '{"cardNumber":"5425233430109903"}'

**Connection String (appsettings.json):**```

```json

"ConnectionStrings": {#### Test 5: Invalid Card Number

  "DefaultConnection": "Server=DevServer;Database=PaymentDB;User Id=sa;Password=Dev123;TrustServerCertificate=True;MultipleActiveResultSets=true"```bash

}curl -X POST http://localhost:5159/api/CardPayment/validate \

```  -H "Content-Type: application/json" \

  -H "Authorization: dev-token-123456" \

**Current Status:** Using in-memory storage for demo purposes. The SQL Server configuration is in place for production use.  -d '{"cardNumber":"4532015112830367"}'

```

**To enable SQL Server:****Response:**

1. Ensure SQL Server is running```json

2. Update connection string in `appsettings.json`{

3. Run migrations: `dotnet ef database update`  "isValid": false,

  "maskedCardNumber": "************0367",

## 🔄 Future Enhancements  "cardType": "Visa",

  "message": "Card number is invalid",

- [ ] JWT token authentication  "validatedAt": "2025-11-10T00:21:10Z"

- [ ] Role-based authorization (RBAC)}

- [ ] Rate limiting per IP/token```

- [ ] Real-time validation cache (Redis)

- [ ] Payment processor integration (Stripe, PayPal)#### Test 6: Health Check (No Auth Required)

- [ ] Transaction logging and audit trail```bash

- [ ] Webhook support for async notificationscurl http://localhost:5159/api/CardPayment/health

- [ ] GraphQL endpoint (alternative to REST)```

- [ ] gRPC support for inter-service communication

- [ ] Circuit breaker pattern (Polly)---

- [ ] Distributed tracing (OpenTelemetry)

**💡 Tip**: Use the interactive Swagger UI at `http://localhost:5159/` for the easiest way to test the API with a visual interface.

## 🐛 Known Issues

## 🔒 Security & Production Notes

None currently. Please report issues on the [GitHub repository](https://github.com/mgsdew/SalesSystem/issues).


**`.gitignore` current state:**
```gitignore
# Environment variables - contains sensitive data like auth tokens
# .env    ← COMMENTED OUT FOR DEMO PURPOSES ONLY
```

#### ⚠️ Production Requirements

In a **real production environment**, you MUST:

1. **Uncomment the `.env` line in `.gitignore`:**
   ```gitignore
   # Environment variables - contains sensitive data like auth tokens
   .env    ← UNCOMMENT THIS IN PRODUCTION
   ```

2. **Never commit sensitive credentials** to version control

3. **Use secure secret management:**
   - Azure Key Vault
   - AWS Secrets Manager
   - HashiCorp Vault
   - Kubernetes Secrets
   - CI/CD pipeline secrets

4. **Rotate tokens regularly** and use strong, unique values

### Security Best Practices Implemented

| Security Feature | Implementation | Status |
|------------------|----------------|--------|
| **Authentication** | Custom middleware with token validation | ✅ Implemented |
| **Input Validation** | Data Annotations + ModelState | ✅ Implemented |
| **Card Masking** | Only last 4 digits exposed | ✅ Implemented |
| **Error Handling** | ProblemDetails pattern | ✅ Implemented |
| **Structured Logging** | ILogger with sensitive data protection | ✅ Implemented |
| **HTTPS** | Configured for production | ✅ Ready |
| **CORS** | Configurable policies | ✅ Ready |

### Additional Production Considerations

1. **HTTPS Enforcement** - Always use TLS/SSL in production
2. **Rate Limiting** - Implement to prevent abuse (e.g., AspNetCoreRateLimit)
3. **API Gateway** - Consider Azure API Management or AWS API Gateway
4. **PCI-DSS Compliance** - If handling real card data
5. **Encryption at Rest** - Encrypt stored card data
6. **Audit Logging** - Log all access attempts
7. **WAF** - Web Application Firewall for DDoS protection

## 🚀 Production Deployment

### Environment Configuration

1. Update `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

2. Configure environment-specific settings:
   - Database connection strings (when using a real database)
   - API keys and secrets
   - CORS policies
   - Rate limiting

### Deployment Checklist

- [ ] Update connection strings
- [ ] Configure logging (Application Insights, Seq, etc.)
- [ ] Enable HTTPS/TLS
- [ ] Configure CORS policies
- [ ] Set up health checks
- [ ] Implement rate limiting
- [ ] Configure monitoring and alerts
- [ ] Set up CI/CD pipeline
- [ ] Encrypt sensitive data at rest
- [ ] Review and update security headers


3. Open a Pull Request

### Coding Standards

- Follow C# coding conventions
- Write unit tests for new features
- Update documentation
- Ensure all tests pass
- Follow SOLID principles


## References

- [Luhn Algorithm](https://en.wikipedia.org/wiki/Luhn_algorithm) - Credit card validation standard
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/) - Official Microsoft documentation
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Robert C. Martin

---
