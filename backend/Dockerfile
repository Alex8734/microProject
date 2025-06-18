FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mx/Mx.csproj", "Mx/"]
COPY ["Mx.Core/Mx.Core.csproj", "Mx.Core/"]
COPY ["Mx.Persistence/Mx.Persistence.csproj", "Mx.Persistence/"]
COPY ["Mx.Shared/Mx.Shared.csproj", "Mx.Shared/"]
RUN dotnet restore "Mx/Mx.csproj"
COPY . .
WORKDIR "/src/Mx"
RUN dotnet build "./Mx.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Migration Schritt: Migrationen bauen und anwenden
FROM build AS migrate
WORKDIR /src/Mx
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef database update --no-build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mx.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=migrate /src/Mx/. .
COPY --from=publish /app/publish .

# Docker CLI installieren
USER root
RUN apt-get update && apt-get install -y docker.io && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "Mx.dll"]
