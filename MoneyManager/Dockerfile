# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY MoneyManager.sln ./
COPY MoneyManager/MoneyManager.csproj MoneyManager/
COPY MoneyManager.Domain/MoneyManager.Domain.csproj MoneyManager.Domain/
COPY MoneyManager.Application/MoneyManager.Application.csproj MoneyManager.Application/
COPY MoneyManager.Infrastructure/MoneyManager.Infrastructure.csproj MoneyManager.Infrastructure/

RUN dotnet restore

# Copy all source code
COPY . .

# Publish the web project
RUN dotnet publish MoneyManager/MoneyManager.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "MoneyManager.dll"]
