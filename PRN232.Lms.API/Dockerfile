FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PRN232.Lms.API/PRN232.Lms.API.csproj", "PRN232.Lms.API/"]
COPY ["PRN232.Lms.Services/PRN232.Lms.Services.csproj", "PRN232.Lms.Services/"]
COPY ["PRN232.Lms.Repositories/PRN232.Lms.Repositories.csproj", "PRN232.Lms.Repositories/"]
RUN dotnet restore "PRN232.Lms.API/PRN232.Lms.API.csproj"

COPY . .
WORKDIR "/src/PRN232.Lms.API"
RUN dotnet build "PRN232.Lms.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PRN232.Lms.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.Lms.API.dll"]
