using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public interface IComponent
{
    public abstract static string Name();
    public abstract static MessageComponent Create(string[] args);
    public abstract static Task HandleAsync(DiscordSocketClient client, SocketMessageComponent component);
}