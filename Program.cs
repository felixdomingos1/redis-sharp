using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

class Program
{
    static async Task Main(string[] args)
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("redis");
        var db = redis.GetDatabase();

        await db.KeyDeleteAsync("bikes:1");
        await db.KeyDeleteAsync("bikes:2");

        await db.HashSetAsync("bikes:1", new[]
        {
            new HashEntry("model", "model1"),
            new HashEntry("price", 1)
        });
        await db.HashSetAsync("bikes:2", new[]
        {
            new HashEntry("model", "model2"),
            new HashEntry("price", 2)
        });
        await db.KeyExpireAsync("bikes:1", TimeSpan.FromSeconds(1.5));

        var timer1 = new Timer(TimerCallback, ("bikes:1", db), 500, Timeout.Infinite);
        var timer2 = new Timer(TimerCallback, ("bikes:2", db), 2000, Timeout.Infinite);

        var resetEvent = new ManualResetEvent(false);
        resetEvent.WaitOne();

        await redis.CloseAsync();
    }
  
    static async void TimerCallback(object state)
    {
        var (key, db) = ((string, IDatabase))state;

        Console.WriteLine("<<<");
        var bike = await db.HashGetAllAsync(key);
        Console.WriteLine($"({key})");
        Console.WriteLine(string.Join("\n", bike.Select(b => $"{b.Name}: {b.Value}")));
        Console.WriteLine(">>>");
    }
}
