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
        
        // appsettings.json を設定していない場合はコメントアウト
        // builder.WebHost.ConfigureKestrel(options =>
        // {
        //     // HTTP/2 Only のエンドポイント（not HTTPS）
        //     options.Listen(IPAddress.Parse("0.0.0.0"), 5000,
        //         listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
        //
        //     // HTTP/2 ,HTTPS エンドポイントの設定
        //     options.Listen(IPAddress.Parse("0.0.0.0"), 5001, listenOptions =>
        //     {
        //         // --load-cert=true が指定されていたら証明書を読み込む
        //         if (args.Any(arg => arg == "--load-cert=true"))
        //         {
        //             Console.WriteLine("load certificate");
        //             listenOptions.UseHttps(new X509Certificate2("certificate/server.pfx", ""));
        //             listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        //         }
        //     });
        //
        //     // 疎通確認用のHTTP 1.1エンドポイントの設定
        //     options.Listen(IPAddress.Parse("0.0.0.0"), 5002,
        //         listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
        // });

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