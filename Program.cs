using StackExchange.Redis;

class Program
{
    static void Main(string[] args)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");

        IDatabase db = redis.GetDatabase();

        db.StringSet("keyA", "A");
        db.StringSet("keyB", "B", TimeSpan.FromSeconds(2));
        db.StringSet("keyC", "C", TimeSpan.FromSeconds(5));

        Timer timer1 = new Timer(TimerCallback, db, 500, Timeout.Infinite);
        Timer timer2 = new Timer(TimerCallback, db, 2000, Timeout.Infinite);
        Timer timer3 = new Timer(TimerCallback, db, 7000, Timeout.Infinite);

        ManualResetEvent resetEvent = new ManualResetEvent(false);
        resetEvent.WaitOne();

        redis.Close();
    }

    static void TimerCallback(object state)
    {
        IDatabase db = (IDatabase)state;

        string valueA = db.StringGet("keyA");
        string valueB = db.StringGet("keyB");
        string valueC = db.StringGet("keyC");

        Console.WriteLine($"Value for keyA: {valueA}");
        Console.WriteLine($"Value for keyB: {valueB}");
        Console.WriteLine($"Value for keyC: {valueC}");
    }
}
