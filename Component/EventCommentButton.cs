using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public class EventCommentButton : IComponent
{
    public static string Name() => "event_comment_button";

    public static MessageComponent Create(string[] args) => new ComponentBuilder()
        .WithButton("Add Comments",
            customId: $"{string.Join('|', [Name(), .. args])}",
            ButtonStyle.Primary
        ).Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketMessageComponent component)
    {
        var eventId = component.Data.CustomId.Split('|')[1];
        var modal = Modal.EventComment.Create([eventId]);

        await component.RespondWithModalAsync(modal); // can not DeferAsync
    }
}