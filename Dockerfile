# Etapa 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

# Etapa 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
 
ENTRYPOINT ["dotnet", "HealthWellbeing.dll"]
