using Discord.WebSocket;

namespace Linie.Modal;

public interface IModal
{
    public abstract static string Name();
    public abstract static Discord.Modal Create(string[] args);
    public abstract static Task HandleAsync(DiscordSocketClient client, SocketModal modal);
}