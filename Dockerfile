# Use the .NET 8.0 SDK image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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
COPY DataBaseWebAPI/Template.mdb /app/Template.mdb # Ensure the file is copied
ENTRYPOINT ["dotnet", "DataBaseWebAPI.dll"]
