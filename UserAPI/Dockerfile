# Use the official .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["UserAPI.sln", "."]
COPY ["UserAPI/UserAPI.csproj", "UserAPI/"]
COPY ["UserAPI.Tests/User.Test.csproj", "UserAPI.Tests/"]

# Restore dependencies
RUN dotnet restore "UserAPI/UserAPI.csproj"

# Copy the entire source code
COPY . .

# Build the application
WORKDIR "/src/UserAPI"
RUN dotnet build "UserAPI.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "UserAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser:appuser /app
USER appuser

# Copy the published application
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "UserAPI.dll"]