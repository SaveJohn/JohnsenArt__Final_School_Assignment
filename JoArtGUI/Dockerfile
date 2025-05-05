FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JoArtGUI/JoArtGUI.csproj", "JoArtGUI/"]
COPY ["JoArtClassLib/JoArtClassLib.csproj", "JoArtClassLib/"]
RUN dotnet restore "JoArtGUI/JoArtGUI.csproj"
COPY . .
WORKDIR "/src/JoArtGUI"
RUN dotnet build "JoArtGUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JoArtGUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:PublishStaticWebAssets=true


FROM base AS final
WORKDIR /app

# 🔧 Definer APP_UID før bruk
ARG APP_UID=1000

# 📦 Kopier filer og sett riktig eierskap
COPY --from=publish /app/publish .
RUN chown -R ${APP_UID}:${APP_UID} /app

# 🔐 Bytt bruker etter at alt er klart
USER ${APP_UID}

ENTRYPOINT ["dotnet", "JoArtGUI.dll"]
