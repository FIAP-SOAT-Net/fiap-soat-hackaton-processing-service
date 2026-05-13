FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["analyzerService/AnalyzerService.Api/AnalyzerService.Api.csproj", "analyzerService/AnalyzerService.Api/"]
RUN dotnet restore "analyzerService/AnalyzerService.Api/AnalyzerService.Api.csproj"
COPY . .
WORKDIR "/src/analyzerService/AnalyzerService.Api"
RUN dotnet build "./AnalyzerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AnalyzerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnalyzerService.Api.dll"]
