using Discord.WebSocket;

namespace Linie;

public static class Components
{
    private delegate Task ComponentsHandlerDelegate(DiscordSocketClient client, SocketMessageComponent command);

    private static readonly Dictionary<string, ComponentsHandlerDelegate> ComponentsHandlers = new()
    {
        [Component.EventJobSelector.Name()] = Component.EventJobSelector.HandleAsync,
        [Component.EventAvailabilitySelector.Name()] = Component.EventAvailabilitySelector.HandleAsync,
        [Component.EventProgressionSelector.Name()] = Component.EventProgressionSelector.HandleAsync,
        [Component.EventHelperSelector.Name()] = Component.EventHelperSelector.HandleAsync,
        [Component.EventCommentButton.Name()] = Component.EventCommentButton.HandleAsync,
        [Component.EventParticipationButtons.Name()] = Component.EventParticipationButtons.HandleAsync,
    };

    public static async Task HandleAsync(DiscordSocketClient client, SocketMessageComponent component)
    {
        var id = component.Data.CustomId.Split('|')[0];
        
        if (string.IsNullOrEmpty(id))
        {
            await component.RespondAsync("Unknown component.", ephemeral: true);
            return;
        }

        if (!ComponentsHandlers.TryGetValue(id, out var handler)) return;
        await handler.Invoke(client, component);
    }
}