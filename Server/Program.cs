using System.Net;
using System.Security.Cryptography.X509Certificates;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Server;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc();
        builder.Services.AddMagicOnion();

        var app = builder.Build();

        // 疎通確認用のルーティング
        app.MapGet("/", () => "Hello World!");

        // MagicOnion用のルーティング
        app.MapMagicOnionService();

        app.Run();
    }
}