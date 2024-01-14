#pragma warning disable CS1998

using DFrame;
using Microsoft.Extensions.DependencyInjection;
using System.Buffers;

var builder = DFrameApp.CreateBuilder(5555, 5556);
builder.ConfigureWorker(options =>
{
    options.VirtualProcess = 1;
    options.Metadata = new()
    {
        {"MachineName", Environment.MachineName},
        {"ProcessorCount", Environment.ProcessorCount.ToString()}
    };
});

if (args.Length == 0)
{
    // local, run both(host WebUI on http://localhost:portWeb)
    await builder.RunAsync();
}
else if (args[0] == "controller")
{
    // listen http://*:portWeb as WebUI and http://*:portListenWorker as Worker listen gRPC
    await builder.RunControllerAsync();
}
else if (args[0] == "worker")
{
    var controllerHost = args[1] ?? "localhost";
    // worker connect to (controller) address.
    // You can also configure from appsettings.json via builder.ConfigureWorker((ctx, options) => { options.ControllerAddress = "" });
    await builder.RunWorkerAsync($"http://{controllerHost}:5556");
}