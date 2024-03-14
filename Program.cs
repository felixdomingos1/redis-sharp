using StackExchange.Redis;

class Program
{
    static void Main(string[] args)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");

        IDatabase db = redis.GetDatabase();

        db.Execute("FLUSHALL");

        db.HashSet("bikes:1", new HashEntry[]
        {
            new HashEntry("model", "model1"),
            new HashEntry("price", 1)
        });
        db.HashSet("bikes:2", new HashEntry[]
        {
            new HashEntry("model", "model1"),
            new HashEntry("price", 2)
        });
        db.KeyExpire("bikes:1", TimeSpan.FromSeconds(1.5));

        Timer timer1 = new Timer(TimerCallback, db, 500, Timeout.Infinite);
        Timer timer2 = new Timer(TimerCallback, db, 2000, Timeout.Infinite);

        ManualResetEvent resetEvent = new ManualResetEvent(false);
        resetEvent.WaitOne();

        redis.Close();
    }

    static void TimerCallback(object state)
    {
        IDatabase db = (IDatabase)state;

        Console.WriteLine("<<<");
        var bike1 = db.HashGetAll("bikes:1");
        Console.WriteLine("(bikes:1)");
        Console.WriteLine(string.Join("\n", bike1.Select(b => $"{b.Name}: {b.Value}")));
        Console.WriteLine(">>>");

        Console.WriteLine("<<<");
        var bike2 = db.HashGetAll("bikes:2");
        Console.WriteLine("(bikes:2)");
        Console.WriteLine(string.Join("\n", bike2.Select(b => $"{b.Name}: {b.Value}")));
        Console.WriteLine(">>>");
    }
}
