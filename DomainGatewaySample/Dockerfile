#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DomainGatewaySample/DomainGatewaySample.csproj", "DomainGatewaySample/"]
COPY ["Leo.Subtypes.Nuget/Leo.Subtypes.Nuget.csproj", "Leo.Subtypes.Nuget/"]
COPY ["PusherSamplePackage/PusherSamplePackage.csproj", "PusherSamplePackage/"]
COPY ["TcsSamplePackage/TcsSamplePackage.csproj", "TcsSamplePackage/"]
RUN dotnet restore "DomainGatewaySample/DomainGatewaySample.csproj"
COPY . .
WORKDIR "/src/DomainGatewaySample"
RUN dotnet build "DomainGatewaySample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DomainGatewaySample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DomainGatewaySample.dll"]