# ビルド環境
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build-env
WORKDIR /app

# 必要なプロジェクトファイルをコピー
COPY Shared ./Shared
COPY Server ./Server

# appsettings.jsonをコピー
COPY Server/appsettings.json ./Server/

# プロジェクトをビルドして公開
WORKDIR /app/Server
RUN dotnet publish -c Release -o out

# 実行環境
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim
WORKDIR /app

# 実行バイナリと関連ファイルをコピー
COPY --from=build-env /app/Server/out ./
COPY --from=build-env /app/Server/appsettings.json /app/
COPY Server/appsettings.json /app/
COPY Server/certificate/server.pfx /app/certificate/

ENTRYPOINT ["dotnet", "Server.dll"]
