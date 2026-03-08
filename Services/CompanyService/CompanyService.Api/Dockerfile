FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CompanyService.Api/CompanyService.Api.csproj", "CompanyService.Api/"]
COPY ["CompanyService.BLL/CompanyService.BLL.csproj", "CompanyService.BLL/"]
COPY ["CompanyService.DAL/CompanyService.DAL.csproj", "CompanyService.DAL/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "CompanyService.Api/CompanyService.Api.csproj"
COPY . .
WORKDIR "/src/CompanyService.Api"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet build "CompanyService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CompanyService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyService.Api.dll"]
