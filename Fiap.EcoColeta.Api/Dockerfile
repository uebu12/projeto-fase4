# ===================== Build Stage =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Fiap.EcoColeta.Api.csproj", "./"]
RUN dotnet restore "Fiap.EcoColeta.Api.csproj"

COPY . .
RUN dotnet build "Fiap.EcoColeta.Api.csproj" -c Release -o /app/build

# ===================== Publish Stage =====================
FROM build AS publish
RUN dotnet publish "Fiap.EcoColeta.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===================== Runtime Stage =====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.EcoColeta.Api.dll"]
