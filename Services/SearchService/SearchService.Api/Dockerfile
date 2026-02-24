FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SearchService.Api/SearchService.Api.csproj", "SearchService.Api/"]
COPY ["SearchService.BLL/SearchService.BLL.csproj", "SearchService.BLL/"]
COPY ["SearchService.DAL/SearchService.DAL.csproj", "SearchService.DAL/"]
COPY ["SearchService.Contract/SearchService.Contract.csproj", "SearchService.Contract/"]
COPY ["SearchService.ML/SearchService.ML.csproj", "SearchService.ML/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "SearchService.Api/SearchService.Api.csproj"
COPY . .
WORKDIR "/src/SearchService.Api"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet build "SearchService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SearchService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SearchService.Api.dll"]
