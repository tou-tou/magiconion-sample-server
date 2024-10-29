using DFrame;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces;
namespace LoadTest;

public class MagicOnionStreamingHubTest : Workload, IGamingHubReceiver
{
    DateTime beginTime;
    DateTime endTime;
    int executeCount;
    GrpcChannel channel = default!;
    IGamingHub client = default!;

    public override async Task SetupAsync(WorkloadContext context)
    {
        beginTime = DateTime.Now;
        channel = GrpcChannel.ForAddress("http://localhost:5059");
        client = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(channel, this);
        await client.JoinAsync("room", "user", new UnityEngine.Vector3(), new UnityEngine.Quaternion());
        endTime = DateTime.Now;
        executeCount++;
    }

    public override async Task ExecuteAsync(WorkloadContext context)
    {
        await client.MoveAsync(new UnityEngine.Vector3(), new UnityEngine.Quaternion());
    }

    public override async Task TeardownAsync(WorkloadContext context)
    {
        await client.LeaveAsync();
        await client.DisposeAsync();
        await channel.ShutdownAsync();
        channel.Dispose();
    }

    public void OnJoin(Player player)
    {
        Console.WriteLine("Join Player:" + player.Name);
    }

    public void OnLeave(Player player)
    {
        Console.WriteLine("Leave Player:" + player.Name);
    }

    public void OnMove(Player player)
    {
        //Console.WriteLine("Move Player:" + player.Name);
    }
    
    public override Dictionary<string, string>? Complete(WorkloadContext context)
    {
        return new()
        {
            { "begin", beginTime.ToString() },
            { "end", endTime.ToString() },
            { "count", executeCount.ToString() },
        };
    }
}