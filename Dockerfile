FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RpgQuestManager.Api/RpgQuestManager.Api.csproj", "src/RpgQuestManager.Api/"]
RUN dotnet restore "src/RpgQuestManager.Api/RpgQuestManager.Api.csproj"
COPY . .
WORKDIR "/src/src/RpgQuestManager.Api"
RUN dotnet build "RpgQuestManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RpgQuestManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RpgQuestManager.Api.dll"]

