using Newtonsoft.Json;

namespace Linie;

public class Configuration
{
    public string DiscordToken = string.Empty;

    public static Configuration? FromFile(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        var file = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Configuration>(file) ?? null;
    } 
}