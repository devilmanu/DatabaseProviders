#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GettingStarted.AspNetCore.Api/GettingStarted.AspNetCore.Api.csproj", "GettingStarted.AspNetCore.Api/"]
COPY ["Database.Providers/Database.Providers.csproj", "Database.Providers/"]
RUN dotnet restore "GettingStarted.AspNetCore.Api/GettingStarted.AspNetCore.Api.csproj"
COPY . .
WORKDIR "/src/GettingStarted.AspNetCore.Api"
RUN dotnet build "GettingStarted.AspNetCore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GettingStarted.AspNetCore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GettingStarted.AspNetCore.Api.dll"]