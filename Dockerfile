# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["HelpdeskTicketingSystem.sln", "./"]
COPY ["Src/Helpdesk.Domain/Helpdesk.Domain.csproj", "Src/Helpdesk.Domain/"]
COPY ["Src/Helpdesk.Application/Helpdesk.Application.csproj", "Src/Helpdesk.Application/"]
COPY ["Src/Helpdesk.Infrastructure/Helpdesk.Infrastructure.csproj", "Src/Helpdesk.Infrastructure/"]
COPY ["Src/Helpdesk.Api/Helpdesk.Api.csproj", "Src/Helpdesk.Api/"]
COPY ["Tests/Helpdesk.Tests/Helpdesk.Tests.csproj", "Tests/Helpdesk.Tests/"]

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish "Src/Helpdesk.Api/Helpdesk.Api.csproj" -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Helpdesk.Api.dll"]
