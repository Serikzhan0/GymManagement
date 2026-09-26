FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for caching
COPY GymManagementApi.sln* ./
COPY GymManagement.Domain/GymManagement.Domain.csproj GymManagement.Domain/
COPY GymManagement.Application/GymManagement.Application.csproj GymManagement.Application/
COPY GymManagement.Infrastructure/GymManagement.Infrastructure.csproj GymManagement.Infrastructure/
COPY GymManagement.Api/GymManagement.Api.csproj GymManagement.Api/

# Restore dependencies
RUN dotnet restore GymManagementApi.sln

# Copy the remaining source files
COPY . .

# Build and publish the Web API
WORKDIR /src/GymManagement.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GymManagement.Api.dll"]
