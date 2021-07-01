FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

WORKDIR /source
COPY src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj
COPY tests/SmsMfaNotificationService.Api.Tests/SmsMfaNotificationService.Api.Tests.csproj tests/SmsMfaNotificationService.Api.Tests/SmsMfaNotificationService.Api.Tests.csproj
COPY SmsMfaNotificationService.sln SmsMfaNotificationService.sln
RUN dotnet restore

WORKDIR /source
COPY . .
RUN set -ex \
    && dotnet --version \
    && dotnet build --configuration Release \
    && dotnet test --configuration Release \ 
    && dotnet publish src/SmsMfaNotificationService.Api/SmsMfaNotificationService.Api.csproj --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "SmsMfaNotificationService.Api.dll"]