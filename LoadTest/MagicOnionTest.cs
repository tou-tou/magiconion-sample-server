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
    private int _roomCount;
    private string _roomName;
    
    GrpcChannel channel = default!;
    IGamingHub client = default!;
    
    public MagicOnionStreamingHubTest(int roomCount)
    {
        _roomCount = roomCount;
    }
    
    // workloadIdを元に部屋を割り当てる
    // ただし、均等に割り当てることを保証しない
    private string AllocateRoom(string workerId)
    {
        int hash = workerId.GetHashCode();
        int roomIndex = Math.Abs(hash) % _roomCount;
        return "room_" + roomIndex;
    }
    
    public override async Task SetupAsync(WorkloadContext context)
    {
        var workloadId = context.WorkloadId.ToString();
        
        beginTime = DateTime.Now;
        channel = GrpcChannel.ForAddress("http://10.0.0.10:5000");
        client = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(channel, this);
        
        _roomName = AllocateRoom(workloadId);
        await client.JoinAsync(_roomName, "user_" + workloadId, new UnityEngine.Vector3(), new UnityEngine.Quaternion());
        endTime = DateTime.Now;
    }

    public override async Task ExecuteAsync(WorkloadContext context)
    {
        executeCount++;
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
        Console.WriteLine("Move Player:" + player.Name);
    }
    
    public override Dictionary<string, string>? Complete(WorkloadContext context)
    {
        return new()
        {
            { "begin", beginTime.ToString() },
            { "end", endTime.ToString() },
            { "count", executeCount.ToString() },
            {"workloadId", context.WorkloadId.ToString()},
            {"workloadIndex", context.WorkloadIndex.ToString()},
            {"ExecuteCount", executeCount.ToString()},
            {"RoomName", _roomName},
        };
    }
}