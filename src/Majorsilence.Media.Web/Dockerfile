#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Majorsilence.Media.Web/Majorsilence.Media.Web.csproj", "Majorsilence.Media.Web/"]
COPY ["Majorsilence.Media.Videos/Majorsilence.Media.Videos.csproj", "Majorsilence.Media.Videos/"]
COPY ["Majorsilence.Media.Images/Majorsilence.Media.Images.csproj", "Majorsilence.Media.Images/"]
RUN dotnet restore "Majorsilence.Media.Web/Majorsilence.Media.Web.csproj"
COPY . .
WORKDIR "/src/Majorsilence.Media.Web"
RUN dotnet build "Majorsilence.Media.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Majorsilence.Media.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Majorsilence.Media.Web.dll"]