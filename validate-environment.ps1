# SalesSystem Environment Validation Script
# Run this script to validate your environment configuration

param(
    [string]$Environment = "Development",
    [switch]$FixIssues,
    [switch]$GenerateKeys
)

Write-Host "🔍 SalesSystem Environment Validator" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "=================================" -ForegroundColor Cyan

# Check if .env file exists
if (!(Test-Path ".env")) {
    Write-Host "❌ .env file not found!" -ForegroundColor Red
    if ($FixIssues) {
        Write-Host "📝 Creating .env file from template..." -ForegroundColor Yellow
        Copy-Item ".env.example" ".env"
        Write-Host "✅ .env file created. Please edit it with your values." -ForegroundColor Green
    }
    exit 1
}

# Load environment variables
Write-Host "📖 Loading environment variables..." -ForegroundColor Yellow
try {
    $envContent = Get-Content ".env" -Raw
    # Parse .env file (basic parsing)
    $envVars = @{}
    foreach ($line in ($envContent -split "`n")) {
        if ($line -match '^([^#][^=]+)=(.*)$') {
            $key = $matches[1].Trim()
            $value = $matches[2].Trim()
            $envVars[$key] = $value
        }
    }
} catch {
    Write-Host "❌ Error reading .env file: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Validation functions
function Test-DatabaseConnection {
    param([string]$connectionString)

    Write-Host "🔍 Testing database connection..." -ForegroundColor Yellow
    try {
        $sqlConnection = New-Object System.Data.SqlClient.SqlConnection
        $sqlConnection.ConnectionString = $connectionString
        $sqlConnection.Open()
        $sqlConnection.Close()
        Write-Host "✅ Database connection successful" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "❌ Database connection failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-ServiceHealth {
    param([string]$url, [string]$serviceName)

    Write-Host "🔍 Testing $serviceName health..." -ForegroundColor Yellow
    try {
        $response = Invoke-WebRequest -Uri "$url/health" -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $serviceName is healthy" -ForegroundColor Green
            return $true
        } else {
            Write-Host "⚠️ $serviceName returned status $($response.StatusCode)" -ForegroundColor Yellow
            return $false
        }
    } catch {
        Write-Host "❌ $serviceName health check failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-TokenEncryptionKey {
    param([string]$key)

    Write-Host "🔍 Validating token encryption key..." -ForegroundColor Yellow
    if ($key.Length -ne 32) {
        Write-Host "❌ Token encryption key must be exactly 32 characters (current: $($key.Length))" -ForegroundColor Red
        if ($GenerateKeys) {
            $newKey = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 32 | ForEach-Object {[char]$_})
            Write-Host "🔑 Generated new key: $newKey" -ForegroundColor Green
            Write-Host "📝 Update TOKEN_ENCRYPTION_KEY_$($Environment.ToUpper()) in .env file" -ForegroundColor Yellow
        }
        return $false
    } else {
        Write-Host "✅ Token encryption key is valid" -ForegroundColor Green
        return $true
    }
}

# Get environment-specific variables
$envSuffix = switch ($Environment) {
    "Development" { "_DEV" }
    "Staging" { "_STAGING" }
    "Production" { "_PROD" }
    default { "_DEV" }
}

# Validate critical variables
$validationResults = @{}

# Check database connection
$dbConnectionString = $envVars["DB_CONNECTION_STRING$envSuffix"]
if ($dbConnectionString) {
    $validationResults["Database"] = Test-DatabaseConnection $dbConnectionString
} else {
    Write-Host "⚠️ Database connection string not found for $Environment" -ForegroundColor Yellow
}

# Check token encryption key
$tokenKey = $envVars["TOKEN_ENCRYPTION_KEY$envSuffix"]
if ($tokenKey) {
    $validationResults["TokenEncryption"] = Test-TokenEncryptionKey $tokenKey
} else {
    Write-Host "⚠️ Token encryption key not found for $Environment" -ForegroundColor Yellow
}

# Check service URLs
$userApiUrl = $envVars["USERAPI_URL$envSuffix"]
$paymentApiUrl = $envVars["PAYMENTAPI_URL$envSuffix"]

if ($userApiUrl) {
    $validationResults["UserAPIService"] = Test-ServiceHealth $userApiUrl "UserAPI"
}

if ($paymentApiUrl) {
    $validationResults["PaymentAPIService"] = Test-ServiceHealth $paymentApiUrl "PaymentAPI"
}

# Check authentication tokens
$authTokens = $envVars["AUTH_VALID_TOKENS$envSuffix"]
if ($authTokens) {
    Write-Host "🔍 Checking authentication tokens..." -ForegroundColor Yellow
    $tokenCount = ($authTokens -split ",").Count
    Write-Host "✅ Found $tokenCount authentication token(s)" -ForegroundColor Green
    $validationResults["AuthTokens"] = $true
} else {
    Write-Host "⚠️ Authentication tokens not found for $Environment" -ForegroundColor Yellow
    $validationResults["AuthTokens"] = $false
}

# Summary
Write-Host "`n📊 Validation Summary" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

$allPassed = $true
foreach ($test in $validationResults.Keys) {
    $status = if ($validationResults[$test]) { "✅ PASS" } else { "❌ FAIL" }
    $color = if ($validationResults[$test]) { "Green" } else { "Red" }
    Write-Host "$status $test" -ForegroundColor $color
    if (!$validationResults[$test]) { $allPassed = $false }
}

Write-Host "`n🎯 Overall Status: $(if ($allPassed) { '✅ ALL CHECKS PASSED' } else { '❌ ISSUES FOUND' })" -ForegroundColor $(if ($allPassed) { "Green" } else { "Red" })

if (!$allPassed) {
    Write-Host "`n💡 Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Review and fix the failed checks above" -ForegroundColor Yellow
    Write-Host "2. Update your .env file with correct values" -ForegroundColor Yellow
    Write-Host "3. Run this script again: .\validate-environment.ps1" -ForegroundColor Yellow
    Write-Host "4. Check ENVIRONMENT_SETUP.md for detailed instructions" -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "`n🚀 Environment is ready for $Environment deployment!" -ForegroundColor Green
}