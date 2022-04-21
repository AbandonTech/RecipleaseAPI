FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Reciplease.csproj", "./"]
RUN dotnet restore "Reciplease.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Reciplease.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reciplease.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reciplease.dll"]
