using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public class EventParticipationButtons : IComponent
{
    public static string Name() => "event_participation_buttons";

    public static MessageComponent Create(string[] args) => new ComponentBuilder()
        .WithButton("Join",
            customId: $"{string.Join('|', [Name(), "join", .. args])}",
            ButtonStyle.Primary)
        .WithButton("Withdraw",
            customId: $"{string.Join('|', [Name(), "withdraw", .. args])}",
            ButtonStyle.Danger)
        .Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketMessageComponent component)
    {
        await component.DeferAsync(ephemeral: true);

        var type = component.Data.CustomId.Split('|')[1];
        var id = component.Data.CustomId.Split('|')[2];

        var @event = await EventManager.GetEventAsync(id) ?? throw new Exception($"Event {id} not found.");
        if (@event.Open == false) throw new Exception($"Event {id} is not open.");
        if (!@event.Participants.TryGetValue(component.User.Id, out var participant))
        {
            participant = new(client, component);
            @event.Participants[component.User.Id] = participant;
        }

        string response = type switch
        {
            "join" => await HandleJoinAsync(participant),
            "withdraw" => await HandleWithdrawAsync(participant),
            _ => throw new Exception($"Unknown action type: {type}")
        };

        await component.FollowupAsync(response, ephemeral: true);
    }

    private static async Task<string> HandleJoinAsync(EventParticipant participant)
    {
        var (canJoin, reason) = participant.State();

        if (!canJoin) return $"You failed to join, {reason}";

        if (participant.Joined) return "You were already participating";

        participant.Joined = true;
        await EventManager.SaveAsync();

        return "You have successfully joined the event";
    }

    private static async Task<string> HandleWithdrawAsync(EventParticipant participant)
    {
        if (!participant.Joined) return "You were not participating, nothing to withdraw.";

        participant.Joined = false;
        await EventManager.SaveAsync();

        return "You have successfully withdrawn from the event.";
    }
}
