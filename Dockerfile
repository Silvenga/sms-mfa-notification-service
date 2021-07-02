FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

WORKDIR /source
COPY src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj
COPY tests/SmsMfaNotificationService.Api.Tests/SmsMfaNotificationService.Api.Tests.csproj tests/SmsMfaNotificationService.Api.Tests/SmsMfaNotificationService.Api.Tests.csproj
RUN set -ex \
    && dotnet restore src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj \
    && dotnet restore tests/SmsMfaNotificationService.Api.Tests/SmsMfaNotificationService.Api.Tests.csproj

WORKDIR /source
COPY . .
RUN set -ex \
    && dotnet --version \
    && dotnet build --configuration Release src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj \
    && dotnet test --configuration Release tests/SmsMfaNotificationService.Api.Tests/SmsMfaNotificationService.Api.Tests.csproj \
    && dotnet publish src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "SmsMfaNotificationService.Api.dll"]
