using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public interface ICommand
{
    public abstract static string Name();
    public abstract static SlashCommandProperties Create();
    public abstract static Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command);
}