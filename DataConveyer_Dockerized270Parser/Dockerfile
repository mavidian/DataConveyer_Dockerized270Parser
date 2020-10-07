FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DataConveyer_Dockerized270Parser/DataConveyer_Dockerized270Parser.csproj", "DataConveyer_Dockerized270Parser/"]
RUN dotnet restore "DataConveyer_Dockerized270Parser/DataConveyer_Dockerized270Parser.csproj"
COPY . .
WORKDIR "/src/DataConveyer_Dockerized270Parser"
RUN dotnet build "DataConveyer_Dockerized270Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataConveyer_Dockerized270Parser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataConveyer_Dockerized270Parser.dll"]