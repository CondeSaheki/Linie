using System.Text;
using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public class EventInfo : ICommand
{
    public static string Name() => "event_info";

    public static SlashCommandProperties Create() => new SlashCommandBuilder()
        .WithName(Name())
        .WithDescription("Show info of an event")
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

        string[] AllJobs =
        [
            "PLD", "WAR", "DRK", "GNB",
            "WHM", "SCH", "AST", "SGE",
            "DRG", "MNK", "NIN", "SAM", "RPR", "VPR",
            "BRD", "MCH", "DNC",
            "BLM", "SMN", "RDM", "PCT",
        ];

        var jobCounts = AllJobs.ToDictionary(job => job, job => 0);

        foreach (var participant in participants)
        {
            foreach (var job in participant.Jobs)
            {
                if (jobCounts.TryGetValue(job, out int value)) jobCounts[job] = ++value;
            }
        }

        var message = new StringBuilder();
        message.AppendLine($"**Title:** {@event.Title}");
        message.AppendLine($"**Description:** {@event.Description}");
        message.AppendLine($"**Open:** {(@event.Open ? "Yes" : "No")}");
        message.AppendLine($"**Participants:** {participants.Length}");
        message.AppendLine("**Job Counts:**");

        message.AppendLine("```");
        foreach (var kvp in jobCounts)
        {
            message.AppendLine($" {kvp.Value,-3} | {kvp.Key}");
        }
        message.AppendLine("```");

        await command.FollowupAsync(message.ToString());

    }
}