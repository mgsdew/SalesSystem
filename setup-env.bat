@echo off
REM SalesSystem Environment Setup Helper
REM This script helps set up environment variables for different environments

echo ========================================
echo SalesSystem Environment Setup Helper
echo ========================================

if "%1"=="dev" goto :setup_dev
if "%1"=="staging" goto :setup_staging
if "%1"=="prod" goto :setup_prod
if "%1"=="check" goto :check_env
if "%1"=="help" goto :show_help

:show_help
echo Usage: setup-env.bat [command]
echo.
echo Commands:
echo   dev      - Setup development environment
echo   staging  - Setup staging environment
echo   prod     - Setup production environment
echo   check    - Check current environment configuration
echo   help     - Show this help message
echo.
echo Examples:
echo   setup-env.bat dev
echo   setup-env.bat check
goto :end

:setup_dev
echo Setting up Development environment...
if not exist ".env" (
    echo Creating .env file from template...
    copy .env.example .env
    echo Please edit .env file with your development values
) else (
    echo .env file already exists
)
set ASPNETCORE_ENVIRONMENT=Development
echo Development environment configured
goto :end

:setup_staging
echo Setting up Staging environment...
if exist ".env.staging" (
    echo Copying staging configuration...
    copy .env.staging .env
    set ASPNETCORE_ENVIRONMENT=Staging
    echo Staging environment configured
) else (
    echo .env.staging not found. Please create it first.
)
goto :end

:setup_prod
echo Setting up Production environment...
if exist ".env.production" (
    echo Copying production configuration...
    copy .env.production .env
    set ASPNETCORE_ENVIRONMENT=Production
    echo Production environment configured
) else (
    echo .env.production not found. Please create it first.
)
goto :end

:check_env
echo Checking environment configuration...
if not exist ".env" (
    echo ERROR: .env file not found!
    echo Run 'setup-env.bat dev' to create it
    goto :end
)

echo Current environment variables:
echo ===============================

REM Check critical variables
if defined AUTH_VALID_TOKENS (
    echo ✅ AUTH_VALID_TOKENS is set
) else (
    echo ❌ AUTH_VALID_TOKENS is not set
)

if defined DB_CONNECTION_STRING (
    echo ✅ DB_CONNECTION_STRING is set
) else (
    echo ❌ DB_CONNECTION_STRING is not set
)

if defined USERAPI_URL (
    echo ✅ USERAPI_URL is set
) else (
    echo ❌ USERAPI_URL is not set
)

if defined PAYMENTAPI_URL (
    echo ✅ PAYMENTAPI_URL is set
) else (
    echo ❌ PAYMENTAPI_URL is not set
)

if defined TOKEN_ENCRYPTION_KEY (
    echo ✅ TOKEN_ENCRYPTION_KEY is set
) else (
    echo ❌ TOKEN_ENCRYPTION_KEY is not set
)

echo.
echo Environment check complete
goto :end

:end
echo.
echo For detailed setup instructions, see ENVIRONMENT_SETUP.md