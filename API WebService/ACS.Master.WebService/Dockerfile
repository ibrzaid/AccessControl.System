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
COPY [".NET Core/API WebService/ACS.Master.WebService/ACS.Master.WebService.csproj", ".NET Core/API WebService/ACS.Master.WebService/"]
COPY ["Common/ACS.License.Validator/ACS.License.Validator.csproj", "Common/ACS.License.Validator/"]
COPY ["Common/ACS.License.Certificate/ACS.License.Certificate.csproj", "Common/ACS.License.Certificate/"]
COPY ["Common/ACS.License/ACS.License.csproj", "Common/ACS.License/"]
COPY ["DataLayer/Database/Connection/ACS.Database.Connection/ACS.Database.Connection.csproj", "DataLayer/Database/Connection/ACS.Database.Connection/"]
COPY ["DataLayer/Database/Connection/ACS.Database.IConnection/ACS.Database.IConnection.csproj", "DataLayer/Database/Connection/ACS.Database.IConnection/"]
COPY [".NET Core/ACS.Background/ACS.Background.csproj", ".NET Core/ACS.Background/"]
COPY [".NET Core/ACS.Models/ACS.Models.csproj", ".NET Core/ACS.Models/"]
COPY [".NET Core/ACS.Service/ACS.Service.csproj", ".NET Core/ACS.Service/"]
COPY ["DataLayer/Database/DataAccess/ACS.Database.DataAccess/ACS.Database.DataAccess.csproj", "DataLayer/Database/DataAccess/ACS.Database.DataAccess/"]
COPY ["DataLayer/Database/Query/ACS.Database.Query/ACS.Database.Query.csproj", "DataLayer/Database/Query/ACS.Database.Query/"]
COPY ["DataLayer/Database/Query/ACS.Database.IQuery/ACS.Database.IQuery.csproj", "DataLayer/Database/Query/ACS.Database.IQuery/"]
COPY ["DataLayer/Database/DataAccess/ACS.Database.IDataAccess/ACS.Database.IDataAccess.csproj", "DataLayer/Database/DataAccess/ACS.Database.IDataAccess/"]
COPY ["DataLayer/ACS.BusinessEntities/ACS.BusinessEntities.csproj", "DataLayer/ACS.BusinessEntities/"]
COPY [".NET Core/ACS.Helper/ACS.Helper.csproj", ".NET Core/ACS.Helper/"]
COPY [".NET Core/ACS.Cache.Service/ACS.Cache.Service.csproj", ".NET Core/ACS.Cache.Service/"]
COPY [".NET Core/ACS.Handler/ACS.Handler.csproj", ".NET Core/ACS.Handler/"]
COPY [".NET Core/ACS.HealthChecks/ACS.HealthChecks.csproj", ".NET Core/ACS.HealthChecks/"]
COPY [".NET Core/ACS.Middleware/ACS.Middleware.csproj", ".NET Core/ACS.Middleware/"]
COPY [".NET Core/ACS.Swagger/ACS.Swagger.csproj", ".NET Core/ACS.Swagger/"]
RUN dotnet restore "./.NET Core/API WebService/ACS.Master.WebService/ACS.Master.WebService.csproj"
COPY . .
WORKDIR "/src/.NET Core/API WebService/ACS.Master.WebService"
RUN dotnet build "./ACS.Master.WebService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ACS.Master.WebService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACS.Master.WebService.dll"]