FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
WORKDIR /app
EXPOSE 8080
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["AtonTest/AtonTest.csproj", "AtonTest/"]
RUN dotnet restore "AtonTest/AtonTest.csproj" --disable-parallel --no-cache

COPY . .
WORKDIR "/src/AtonTest"
RUN dotnet build "AtonTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AtonTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AtonTest.dll"]