# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY KudosApp.slnx .
COPY src/KudosApp.Domain/KudosApp.Domain.csproj src/KudosApp.Domain/
COPY src/KudosApp.Application/KudosApp.Application.csproj src/KudosApp.Application/
COPY src/KudosApp.Infrastructure/KudosApp.Infrastructure.csproj src/KudosApp.Infrastructure/
COPY src/KudosApp.Api/KudosApp.Api.csproj src/KudosApp.Api/

RUN dotnet restore src/KudosApp.Api/KudosApp.Api.csproj

COPY src/ src/
WORKDIR /src/src/KudosApp.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

RUN mkdir -p /app/data

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "KudosApp.Api.dll"]
