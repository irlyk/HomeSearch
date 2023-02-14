#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HomeSearch/Server/HomeSearch.Server.csproj", "HomeSearch/Server/"]
COPY ["HomeSearch/Client/HomeSearch.Client.csproj", "HomeSearch/Client/"]
COPY ["HomeSearch/Shared/HomeSearch.Shared.csproj", "HomeSearch/Shared/"]
RUN dotnet restore "HomeSearch/Server/HomeSearch.Server.csproj"
COPY . .
WORKDIR "/src/HomeSearch/Server"
RUN dotnet build "HomeSearch.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeSearch.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeSearch.Server.dll"]
