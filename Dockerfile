# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Plant-Explorer/Plant-Explorer.csproj", "Plant-Explorer/"]
COPY ["Plant-Explorer.Repositories/Plant-Explorer.Repositories.csproj", "Plant-Explorer.Repositories/"]
COPY ["Plant-Explorer.Contract.Repositories/Plant-Explorer.Contract.Repositories.csproj", "Plant-Explorer.Contract.Repositories/"]
COPY ["Plant-Explorer.Core/Plant-Explorer.Core.csproj", "Plant-Explorer.Core/"]
COPY ["Plant-Explorer.Services/Plant-Explorer.Services.csproj", "Plant-Explorer.Services/"]
COPY ["Plant-Explorer.Contract.Services/Plant-Explorer.Contract.Services.csproj", "Plant-Explorer.Contract.Services/"]
COPY ["Plant-Explorer.Mapper/Plant-Explorer.Mapper.csproj", "Plant-Explorer.Mapper/"]
RUN dotnet restore "./Plant-Explorer/Plant-Explorer.csproj"
COPY . .
WORKDIR "/src/Plant-Explorer"
RUN dotnet build "./Plant-Explorer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Plant-Explorer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plant-Explorer.dll"]