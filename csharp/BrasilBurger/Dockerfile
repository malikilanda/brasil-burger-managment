# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie le projet et restore
COPY . .
RUN dotnet restore

# Publish
RUN dotnet publish -c Release -o /app/publish

# ---- Run ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render expose un PORT (souvent 10000)
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

EXPOSE 10000

ENTRYPOINT ["dotnet", "BrasilBurger.dll"]
