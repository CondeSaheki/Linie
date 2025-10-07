using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public class Ping : ICommand
{
    public static string Name() => "ping";

    public static SlashCommandProperties Create() => new SlashCommandBuilder()
        .WithName(Name())
        .WithDescription("Gets this app latency")
        .Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);
        await command.FollowupAsync($"My current ping is {client.Latency}ms");
    }
}
