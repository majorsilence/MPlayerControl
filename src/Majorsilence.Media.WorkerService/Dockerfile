#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Majorsilence.Media.WorkerService/Majorsilence.Media.WorkerService.csproj", "Majorsilence.Media.WorkerService/"]
RUN dotnet restore "Majorsilence.Media.WorkerService/Majorsilence.Media.WorkerService.csproj"
COPY . .
WORKDIR "/src/Majorsilence.Media.WorkerService"
RUN dotnet build "Majorsilence.Media.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Majorsilence.Media.WorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# USER app is the default starting with aspnet:8.0 images
# See https://devblogs.microsoft.com/dotnet/running-nonroot-kubernetes-with-dotnet/ and 
# https://devblogs.microsoft.com/dotnet/securing-containers-with-rootless/
USER $APP_UID
ENTRYPOINT ["dotnet", "Majorsilence.Media.WorkerService.dll"]