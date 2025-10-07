using System.Text;
using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public class EventExport : ICommand
{
    public static string Name() => "event_export";

    public static SlashCommandProperties Create() => new SlashCommandBuilder()
        .WithName(Name())
        .WithDescription("Export an event participants in CSV format")
        .AddOption("id", ApplicationCommandOptionType.String, "Event Id", isRequired: true)
        .Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);

        var id = (string)command.Data.Options.First().Value!;
        var @event = await EventManager.GetEventAsync(id);

        if (@event == null)
        {
            await command.FollowupAsync($"Event id `{id}` not found", ephemeral: true);
            return;
        }
        
        var participants = @event.Participants
            .Where((participant) => participant.Value.Joined)
            .Select((pair) => pair.Value)
            .ToArray();

        // string json = JsonConvert.SerializeObject(participants, Formatting.Indented);
        var csv = EventParticipant.AsCsv(participants);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv))
        {
            Position = 0
        };

        await command.FollowupWithFileAsync(stream, $"{id}_participants.csv", ephemeral: true).ContinueWith((_) => stream.Dispose());
    }
}
