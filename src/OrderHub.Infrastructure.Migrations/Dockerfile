FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["src/OrderHub.Infrastructure.Migrations/OrderHub.Infrastructure.Migrations.csproj", "src/OrderHub.Infrastructure.Migrations/"]
COPY ["src/OrderHub.Infrastructure/OrderHub.Infrastructure.csproj", "src/OrderHub.Infrastructure/"]
COPY ["src/OrderHub.Application/OrderHub.Application.csproj", "src/OrderHub.Application/"]
COPY ["src/OrderHub.Domain/OrderHub.Domain.csproj", "src/OrderHub.Domain/"]
RUN dotnet restore "src/OrderHub.Infrastructure.Migrations/OrderHub.Infrastructure.Migrations.csproj"

COPY . .
WORKDIR /src/src/OrderHub.Infrastructure.Migrations
RUN dotnet publish "OrderHub.Infrastructure.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install --no-install-recommends -y libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "OrderHub.Infrastructure.Migrations.dll"]
