using System.Text;
using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public class EventList : ICommand
{
    public static string Name() => "event_list";

    public static SlashCommandProperties Create() => new SlashCommandBuilder()
        .WithName(Name())
        .WithDescription("List all events")
        .Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);

        var message = new StringBuilder();
        var events = await EventManager.GetEventsAsync();

        if (!events.IsEmpty) foreach (var (id, @event) in events)
        {
            var participantCount = @event.Participants.Count(participant => participant.Value.Joined);

            message.AppendLine($"**Id:** {id}");
            message.AppendLine($"**Title:** {@event.Title}");
            message.AppendLine($"**Description:** {@event.Description}");
            message.AppendLine($"**Open:** {(@event.Open ? "Yes" : "No")}");
            message.AppendLine($"**Participants:** {participantCount}");
            message.AppendLine($"\n");
        }
        else message.AppendLine("No Events");
        
        await command.FollowupAsync($"{message}", ephemeral: true);
    }
}