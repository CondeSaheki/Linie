using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Linie;

public class EventManager
{
    private const string FileName = "DataBase";
    private readonly ConcurrentDictionary<string, Event> Events = new();
    private readonly SemaphoreSlim FileLock = new(1, 1);

    private static readonly Lazy<EventManager> instance = new(() => new EventManager());
    private static EventManager Instance => instance.Value;

    private EventManager()
    {
        FileLock.Wait();
        try
        {
            if (!File.Exists(FileName))
            {
                Logger.Write("No existing data file found");
                return;
            }

            var file = File.ReadAllText(FileName);
            var events = JsonConvert.DeserializeObject<Dictionary<string, Event>>(file) ?? [];

            foreach (var (id, @event) in events) Events[id] = @event;

            Logger.Write($"Loaded {Events.Count} events");
        }
        catch (Exception exception)
        {
            Logger.Write($"Failed to load events {exception}");
        }
        finally
        {
            FileLock.Release();
        }
    }

    public static async Task SaveAsync(string id, Event eventData)
    {
        Instance.Events[id] = eventData;
        await SaveAsync();
    }

    public static async Task SaveAsync()
    {
        await Instance.FileLock.WaitAsync();
        try
        {
            var json = JsonConvert.SerializeObject(Instance.Events, Formatting.Indented);
            await File.WriteAllTextAsync(FileName, json);
        }
        catch (Exception exception)
        {
            Logger.Write($"Failed to persist events to file: {exception}");
        }
        finally
        {
            Instance.FileLock.Release();
        }
    }

    public static async Task DeleteAsync(string eventId)
    {
        Instance.Events.TryRemove(eventId, out _);
        await SaveAsync();
    }

    public static Task<Event?> GetEventAsync(string id)
    {
        Instance.Events.TryGetValue(id, out var eventData);
        return Task.FromResult(eventData);
    }

    public static Task<ConcurrentDictionary<string, Event>> GetEventsAsync()
    {
        return Task.FromResult(Instance.Events);
    }
}