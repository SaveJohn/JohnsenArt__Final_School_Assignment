FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JoArtAPI/JoArtAPI.csproj", "JoArtAPI/"]
COPY ["JoArtClassLib/JoArtClassLib.csproj", "JoArtClassLib/"]
COPY ["JoArtDataLayer/JoArtDataLayer.csproj", "JoArtDataLayer/"]
RUN dotnet restore "JoArtAPI/JoArtAPI.csproj"
COPY . .
WORKDIR "/src/JoArtAPI"
RUN dotnet build "JoArtAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JoArtAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JoArtAPI.dll"]
