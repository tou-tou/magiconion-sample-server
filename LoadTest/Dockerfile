# ビルド環境
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build-env
WORKDIR /app

COPY Shared ./Shared
COPY LoadTest ./LoadTest

# プロジェクトをビルドして公開
WORKDIR /app/LoadTest
RUN dotnet publish -c Release -o out

# 実行環境
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
WORKDIR /app
COPY --from=build-env /app/LoadTest/out ./
ENTRYPOINT ["dotnet", "LoadTest.dll"]
