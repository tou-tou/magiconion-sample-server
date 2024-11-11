using System.Diagnostics;
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
    private int _interval;
    private Random _random;
    private string _roomName;
    
    GrpcChannel channel = default!;
    IGamingHub client = default!;
    
    public MagicOnionStreamingHubTest(int roomCount, int interval)
    {
        _roomCount = roomCount;
        _interval = interval;
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
        _random = new Random();
        
        await client.JoinAsync(_roomName, "user_" + workloadId, new UnityEngine.Vector3(), new UnityEngine.Quaternion());
    }

    public override async Task ExecuteAsync(WorkloadContext context)
    {
        var stopWatch = Stopwatch.StartNew();
        executeCount++;
        
        var randomVector = new UnityEngine.Vector3(_random.Next(0, 100), _random.Next(0, 100), _random.Next(0, 100));
        var randomRotation = new UnityEngine.Quaternion(_random.Next(0, 100), _random.Next(0, 100), _random.Next(0, 100), _random.Next(0, 100));
        await client.MoveAsync(randomVector, randomRotation);
        stopWatch.Stop();
        
        var elapseMs = stopWatch.ElapsedMilliseconds;
        var delayMs = _interval - elapseMs;
        
        if (delayMs > 0) await Task.Delay((int)delayMs);
    }

    public override async Task TeardownAsync(WorkloadContext context)
    {
        await client.LeaveAsync();
        await client.DisposeAsync();
        await channel.ShutdownAsync();
        channel.Dispose();
        endTime = DateTime.Now;
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