FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WorkerService.Api/WorkerService.Api.csproj", "WorkerService.Api/"]
COPY ["WorkerService.BLL/WorkerService.BLL.csproj", "WorkerService.BLL/"]
COPY ["WorkerService.DAL/WorkerService.DAL.csproj", "WorkerService.DAL/"]
RUN dotnet restore "WorkerService.Api/WorkerService.Api.csproj"
COPY . .
WORKDIR "/src/WorkerService.Api"
RUN dotnet build "WorkerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WorkerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerService.Api.dll"]
