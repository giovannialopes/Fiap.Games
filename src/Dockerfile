# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Games.API/Games.API.csproj", "Games.API/"]
COPY ["Games.Domain/Games.Domain.csproj", "Games.Domain/"]
COPY ["Games.Infrastructure/Games.Infrastructure.csproj", "Games.Infrastructure/"]
COPY ["Games.Domain.Shared/Games.Domain.Shared.csproj", "Games.Domain.Shared/"]

RUN dotnet restore "Games.API/Games.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Games.API"
RUN rm -rf obj bin && dotnet publish "Games.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=build /app/publish .

# Set ownership and permissions
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Games.API.dll"]
