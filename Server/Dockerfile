FROM mcr.microsoft.com/dotnet/sdk:8.0.100-1-bookworm-slim AS build-env
WORKDIR /app

COPY ./Shared/Shared.csproj ./Shared/
COPY ./Server/Server.csproj ./Server/
RUN dotnet restore ./Server/Server.csproj

COPY ./Shared ./Shared
COPY ./Server ./Server

RUN dotnet publish Server -c Release -o out

# 実行時のイメージ
FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-bookworm-slim
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Server.dll"]

