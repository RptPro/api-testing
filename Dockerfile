# Use the .NET 8.0 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DataBaseWebAPI/DataBaseWebAPI.csproj", "DataBaseWebAPI/"]
RUN dotnet restore "DataBaseWebAPI/DataBaseWebAPI.csproj"
COPY ["DataBaseWebAPI/", "/src/DataBaseWebAPI/"]
WORKDIR "/src/DataBaseWebAPI"
RUN dotnet build "DataBaseWebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DataBaseWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the .NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=publish /app/publish . 

# Copy the Template.mdb file into the correct location
COPY ["DataBaseWebAPI/wwwroot/Template.mdb", "/app/wwwroot/Template.mdb"]

EXPOSE 80
EXPOSE 443

# Set the entry point for the application
ENTRYPOINT ["dotnet", "DataBaseWebAPI.dll"]
