FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Games.API/Games.API.csproj", "Games.API/"]
RUN dotnet restore "Games.API/Games.API.csproj"
COPY . .
WORKDIR "/src/Games.API"
RUN dotnet publish "Games.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Games.API.dll"]
