FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 443
EXPOSE 8081

ENV ASPNETCORE_URLS=http://*:80/;http://*:8080/;https://*:443/;https://*:8081/

# Install packages
# RUN apt update
# RUN apt install tzdata

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ArchiBase.csproj", "./"]
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
RUN dotnet restore "ArchiBase.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ArchiBase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArchiBase.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArchiBase.dll", "--server.urls", "http://+:80/;http://+:8080/;https://+:443/;https://+:8081/"]
