FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Orders.slnx ./
COPY src/Orders.Api/Orders.Api.csproj src/Orders.Api/
COPY src/Orders.Application/Orders.Application.csproj src/Orders.Application/
COPY src/Orders.Domain/Orders.Domain.csproj src/Orders.Domain/
COPY src/Orders.Infrastructure/Orders.Infrastructure.csproj src/Orders.Infrastructure/
RUN dotnet restore ./Orders.slnx

COPY . .
RUN dotnet publish src/Orders.Api/Orders.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

RUN apt-get update \
  && apt-get install -y --no-install-recommends curl \
  && rm -rf /var/lib/apt/lists/* \
  && (getent group app >/dev/null || groupadd --system app) \
  && (id -u app >/dev/null 2>&1 || useradd --system --gid app --home-dir /app --shell /usr/sbin/nologin app) \
  && chown -R app:app /app

COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
USER app
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
