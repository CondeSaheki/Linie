using Discord;
using Discord.WebSocket;

namespace Linie.Modal;

public class EventComment : IModal
{
    public static string Name() => "event_comment_modal";

    public static Discord.Modal Create(string[] args) => new ModalBuilder()
        .WithTitle("Comment")
        .WithCustomId(string.Join('|', [Name(), .. args]))
        .AddTextInput("Your notes",
            "notes",
            TextInputStyle.Paragraph,
            placeholder: "Add any notes or comments",
            required: false
        ).Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketModal modal)
    {
        await modal.DeferAsync(ephemeral: true);

        var id = modal.Data.CustomId.Split('|')[1];

        var @event = await EventManager.GetEventAsync(id) ?? throw new Exception($"Event {id} not found.");
        if (@event.Open == false) throw new Exception($"Event {id} is not open.");
        if (!@event.Participants.TryGetValue(modal.User.Id, out var participant))
        {
            participant = new(client, modal);
            @event.Participants[modal.User.Id] = participant;
        }
        
        var values = modal.Data.Components.FirstOrDefault();
        var comments = values?.Value ?? string.Empty;

        if (participant.Comments == comments) return;

        participant.Comments = comments;
        
        if (participant.Joined)
        {
            participant.Joined = false;
            await modal.FollowupAsync("Updating your `Comments` has canceled your participation, you can rejoin at any time", ephemeral: true);
        }

        await EventManager.SaveAsync();
    }
}