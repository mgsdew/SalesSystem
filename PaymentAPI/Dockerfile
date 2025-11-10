# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["PaymentAPI/PaymentAPI.csproj", "PaymentAPI/"]
COPY ["PaymentAPI.Tests/Payment.Test.csproj", "PaymentAPI.Tests/"]
COPY ["PaymentAPI.sln", "./"]

# Restore dependencies
RUN dotnet restore "PaymentAPI.sln"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/PaymentAPI"
RUN dotnet build "PaymentAPI.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "PaymentAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Run the application
ENTRYPOINT ["dotnet", "PaymentAPI.dll"]
