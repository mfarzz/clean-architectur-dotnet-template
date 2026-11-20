FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/MyCleanApi.Domain/MyCleanApi.Domain.csproj", "src/MyCleanApi.Domain/"]
COPY ["src/MyCleanApi.Application/MyCleanApi.Application.csproj", "src/MyCleanApi.Application/"]
COPY ["src/MyCleanApi.Infrastructure/MyCleanApi.Infrastructure.csproj", "src/MyCleanApi.Infrastructure/"]
COPY ["src/MyCleanApi.WebApi/MyCleanApi.WebApi.csproj", "src/MyCleanApi.WebApi/"]

RUN dotnet restore "src/MyCleanApi.WebApi/MyCleanApi.WebApi.csproj"

COPY . .
WORKDIR "/src/src/MyCleanApi.WebApi"
RUN dotnet build "MyCleanApi.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyCleanApi.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyCleanApi.WebApi.dll"]
