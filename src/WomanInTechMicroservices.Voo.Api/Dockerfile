#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WomanInTechMicroservices.Voo.Api/WomanInTechMicroservices.Voo.Api.csproj", "src/WomanInTechMicroservices.Voo.Api/"]
RUN dotnet restore "src/WomanInTechMicroservices.Voo.Api/WomanInTechMicroservices.Voo.Api.csproj"
COPY . .
WORKDIR "/src/src/WomanInTechMicroservices.Voo.Api"
RUN dotnet build "WomanInTechMicroservices.Voo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WomanInTechMicroservices.Voo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WomanInTechMicroservices.Voo.Api.dll"]