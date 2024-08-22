FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5220
EXPOSE 5221

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Crayon.API.Domain/Crayon.API.Domain.csproj", "Crayon.API.Domain/"]
COPY ["Crayon.API.Infrastructure/Crayon.API.Infrastructure.csproj", "Crayon.API.Infrastructure/"]
COPY ["Crayon.API/Crayon.API.csproj", "Crayon.API/"]
RUN dotnet restore "Crayon.API.Domain/Crayon.API.Domain.csproj"
RUN dotnet restore "Crayon.API.Infrastructure/Crayon.API.Infrastructure.csproj"
RUN dotnet restore "Crayon.API/Crayon.API.csproj"
COPY . .
WORKDIR "/src/Crayon.API"
RUN dotnet build "Crayon.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Crayon.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crayon.API.dll"]
