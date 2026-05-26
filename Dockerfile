FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ComicNew.Api/ComicNew.Api.csproj", "src/ComicNew.Api/"]
COPY ["src/ComicNew.Application/ComicNew.Application.csproj", "src/ComicNew.Application/"]
COPY ["src/ComicNew.Domain/ComicNew.Domain.csproj", "src/ComicNew.Domain/"]
COPY ["src/ComicNew.Infrastructure/ComicNew.Infrastructure.csproj", "src/ComicNew.Infrastructure/"]

RUN dotnet restore "src/ComicNew.Api/ComicNew.Api.csproj"

COPY . .
RUN dotnet publish "src/ComicNew.Api/ComicNew.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet ComicNew.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
