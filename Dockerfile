# ---- 1. aşama: derleme (SDK imajı büyük, sadece burada kullanılır) ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Önce sadece .csproj'ları kopyala: paketler (restore) cache'lenir, kod değişince yeniden inmez.
COPY EMagaza.sln .
COPY src/EMagaza.Domain/*.csproj src/EMagaza.Domain/
COPY src/EMagaza.Application/*.csproj src/EMagaza.Application/
COPY src/EMagaza.Infrastructure/*.csproj src/EMagaza.Infrastructure/
COPY src/EMagaza.Web/*.csproj src/EMagaza.Web/
COPY src/EMagaza.Worker/*.csproj src/EMagaza.Worker/
COPY tests/EMagaza.Tests/*.csproj tests/EMagaza.Tests/
RUN dotnet restore
COPY . .
RUN dotnet publish src/EMagaza.Web -c Release -o /app --no-restore

# ---- 2. aşama: çalışma (küçük runtime imajı) ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
# SQLite dosyası container içinde; kalıcı olması için volume bağlanır (docker-compose.yml)
ENTRYPOINT ["dotnet", "EMagaza.Web.dll"]
