using Discord.WebSocket;

namespace Linie;

public static class Modals
{
    private delegate Task ModalHandlerDelegate(DiscordSocketClient client, SocketModal command);

    private static readonly Dictionary<string, ModalHandlerDelegate> ModalsHandlers = new()
    {
        [Modal.EventComment.Name()] = Modal.EventComment.HandleAsync
    };

    public static async Task HandleAsync(DiscordSocketClient client, SocketModal modal)
    {
        var id = modal.Data.CustomId.Split('|')[0];

        if (string.IsNullOrEmpty(id))
        {
            await modal.RespondAsync("Unknown modal.", ephemeral: true);
            return;
        }

        if (!ModalsHandlers.TryGetValue(id, out var handler)) return;
        await handler.Invoke(client, modal);
    }
}
    