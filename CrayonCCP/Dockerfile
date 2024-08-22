FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5120
EXPOSE 5121

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CrayonCCP.Domain/CrayonCCP.Domain.csproj", "CrayonCCP.Domain/"]
COPY ["CrayonCCP.Infrastructure/CrayonCCP.Infrastructure.csproj", "CrayonCCP.Infrastructure/"]
COPY ["CrayonCCP/CrayonCCP.csproj", "CrayonCCP/"]
RUN dotnet restore "CrayonCCP.Domain/CrayonCCP.Domain.csproj"
RUN dotnet restore "CrayonCCP.Infrastructure/CrayonCCP.Infrastructure.csproj"
RUN dotnet restore "CrayonCCP/CrayonCCP.csproj"
COPY . .
WORKDIR "/src/CrayonCCP"
RUN dotnet build "CrayonCCP.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CrayonCCP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrayonCCP.dll"]
