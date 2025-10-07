using System.Text;
using System.Text.Json.Serialization;
using Discord.WebSocket;

namespace Linie;

public class EventParticipant
{
    public string Name { get; set; } = string.Empty;
    public string Mention { get; set; } = string.Empty;

    private bool joined;
    
    public bool Joined
    {
        get => joined;
        set
        {
            if (joined != value)
            {
                joined = value;
                if (joined) JoinedDateTime = DateTime.UtcNow;
                else JoinedDateTime = null;
            }
        }
    }

    public DateTime? JoinedDateTime { get; set; } = null;

    public List<string> Jobs { get; set; } = [];
    public List<string> Availability { get; set; } = [];
    public string Progression { get; set; } = string.Empty;
    public bool? Helper { get; set; } = null;
    public string Comments { get; set; } = string.Empty;

    [JsonConstructor]
    public EventParticipant() { }

    public EventParticipant(DiscordSocketClient client, SocketInteraction interaction)
    {
        var nickname = interaction.GuildId is ulong guildId
            ? client.GetGuild(guildId).GetUser(interaction.User.Id).Nickname
            : null;

        var name = string.IsNullOrEmpty(nickname)
            ? interaction.User.GlobalName
            : nickname;

        Name = name;
        Mention = interaction.User.Mention;
    }

    // needs rework, should be TryJoin 
    public (bool valid, string reason) State()
    {
        if (Jobs.Count == 0) return (false, "missing job");
        if (Availability.Count == 0) return (false, "missing availability");
        if (Progression == string.Empty) return (false, "missing progression");
        if (Helper == null) return (false, "missing helper");

        return (true, string.Empty);
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n')) return value;

        var replace = value.Replace("\"", "\"\"");
        return $"\"{replace}\"";
    }

    // very especific format, not very flexible
    public static string AsCsv(IEnumerable<EventParticipant> participants)
    {
        var builder = new StringBuilder();

        builder.AppendLine("name,is_helper,is_max_parties,prog_point,availability,jobs,notes");

        foreach (var participant in participants)
        {
            var jobs = participant.Jobs.Count != 1 ?
                Escape(string.Join(", ", participant.Jobs)) :
                participant.Jobs.FirstOrDefault();

            var availability = participant.Availability.Count != 1 ?
                Escape(string.Join(", ", participant.Availability)) :
                participant.Availability.FirstOrDefault();

            var line = string.Join(",",
                Escape(participant.Name),
                (participant.Helper != null && (bool)participant.Helper) ? "TRUE" : "FALSE",
                "FALSE",
                Escape(participant.Progression),
                availability,
                jobs,
                $"{Escape(participant.Comments)} | {Escape(participant.Mention)}"
            );

            builder.AppendLine(line);
        }
        return builder.ToString();
    }
}