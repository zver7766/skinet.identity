FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY ["./app/Skinet.Identity.WebApi/Skinet.Identity.WebApi.csproj", "./Skinet.Identity.WebApi/Skinet.Identity.WebApi.csproj"]
COPY ["./app/Skinet.Identity.Domain/Skinet.Identity.Domain.csproj", "./Skinet.Identity.Domain/Skinet.Identity.Domain.csproj"]
COPY ["./app/Skinet.Identity.Infrastructure/Skinet.Identity.Infrastructure.csproj", "./Skinet.Identity.Infrastructure/Skinet.Identity.Infrastructure.csproj"]
RUN dotnet restore "./Skinet.Identity.WebApi/Skinet.Identity.WebApi.csproj" /t:Restore

COPY ./app .
WORKDIR "/src/Skinet.Identity.WebApi"
RUN dotnet build "Skinet.Identity.WebApi.csproj" -c Release -o /app/build 

FROM build AS publish
RUN dotnet publish "Skinet.Identity.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["/certificates/", "/usr/local/share/ca-certificates/"]
RUN \
    update-ca-certificates && \
    apt-get update -y && \
    apt-get upgrade -y
ENTRYPOINT ["dotnet", "Skinet.Identity.WebApi.dll"]