# Use the .NET 8.0 SDK image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DataBaseWebAPI/DataBaseWebAPI.csproj", "DataBaseWebAPI/"]
RUN dotnet restore "DataBaseWebAPI/DataBaseWebAPI.csproj"
COPY . . 
WORKDIR "/src/DataBaseWebAPI"
RUN dotnet build "DataBaseWebAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "DataBaseWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["DataBaseWebAPI/wwwroot/Template.mdb", "/app/wwwroot/Template.mdb"]
COPY ["DataBaseWebAPI/wwwroot/UsersDB.mdb", "/app/wwwroot/UsersDB.mdb"]

# Add debugging output to log
RUN echo "Starting container..." > /app/startup.log
RUN cat /app/startup.log

ENTRYPOINT ["dotnet", "DataBaseWebAPI.dll"]
