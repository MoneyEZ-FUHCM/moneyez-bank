# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8088
EXPOSE 8089


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MoneyEzBank.API/MoneyEzBank.API.csproj", "MoneyEzBank.API/"]
COPY ["MoneyEzBank.Services/MoneyEzBank.Services.csproj", "MoneyEzBank.Services/"]
COPY ["MoneyEzBank.Repositories/MoneyEzBank.Repositories.csproj", "MoneyEzBank.Repositories/"]
RUN dotnet restore "./MoneyEzBank.API/MoneyEzBank.API.csproj"
COPY . .
WORKDIR "/src/MoneyEzBank.API"
RUN dotnet build "./MoneyEzBank.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MoneyEzBank.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Cấu hình ứng dụng chạy ở cổng 8088
ENV ASPNETCORE_URLS=http://+:8088

# Expose lại cổng 8088 để Docker biết container đang mở cổng này
EXPOSE 8088
ENTRYPOINT ["dotnet", "MoneyEzBank.API.dll"]