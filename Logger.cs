namespace Linie;

public class Logger : IDisposable
{
    private static readonly Lazy<Logger> instance = new(() => new("Linie.log"));
    public static Logger Instance => instance.Value;

    private readonly StreamWriter Writer;
    private readonly Lock WriteLock = new();

    private Logger(string logFilePath)
    {
        Writer = new(logFilePath, true);
    }

    public static void Write(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        var line = $"{DateTime.Now:HH:mm:ss} {message}";
        lock (Instance.WriteLock)
        {
            Console.WriteLine(line);
            Instance.Writer.WriteLine(line);
            Instance.Writer.Flush();
        }
    }

    public static Task WriteAsync(string message)
    {
        return Task.Run(() => Write(message));
    }

    public void Dispose()
    {
        lock (WriteLock)
        {
            Writer?.Dispose();
        }
    }
}
