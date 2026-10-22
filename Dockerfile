FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY GymManagement.sln ./
COPY src/GymManagement.Domain/GymManagement.Domain.csproj src/GymManagement.Domain/
COPY src/GymManagement.Application/GymManagement.Application.csproj src/GymManagement.Application/
COPY src/GymManagement.Persistence/GymManagement.Persistence.csproj src/GymManagement.Persistence/
COPY src/GymManagement.Api/GymManagement.Api.csproj src/GymManagement.Api/

RUN dotnet restore GymManagement.sln

# Copy all source files and publish Api project
COPY . .
RUN dotnet publish src/GymManagement.Api/GymManagement.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GymManagement.Api.dll"]
