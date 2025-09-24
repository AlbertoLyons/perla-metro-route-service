FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /route-service-app

EXPOSE 80
EXPOSE 5003

# COPY csproj and restore as distinct layers
COPY ./*csproj ./
RUN dotnet restore

# COPY everything else and build app
COPY . .
RUN dotnet publish -c Release -o out

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /route-service-app
COPY --from=build /route-service-app/out .
ENTRYPOINT ["dotnet", "perla-metro-route-service.dll"]