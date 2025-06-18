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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mx.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mx.dll"]
