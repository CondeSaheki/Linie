using Discord.WebSocket;

namespace Linie;

public static class Commands
{
    private delegate Task CommandHandlerDelegate(DiscordSocketClient client, SocketSlashCommand command);

    private static readonly Dictionary<string, CommandHandlerDelegate> CommandHandlers = new()
    {
        [Command.EventCreate.Name()] = Command.EventCreate.HandleAsync,
        [Command.EventRemove.Name()] = Command.EventRemove.HandleAsync,
        [Command.EventInfo.Name()] = Command.EventInfo.HandleAsync,
        [Command.EventList.Name()] = Command.EventList.HandleAsync,
        [Command.EventExport.Name()] = Command.EventExport.HandleAsync,
        [Command.Ping.Name()] = Command.Ping.HandleAsync
    };

    public static async Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command)
    {
        if (!CommandHandlers.TryGetValue(command.CommandName, out var handler)) return;
        await handler.Invoke(client, command);
    }

    public static async Task RegisterAsync(DiscordSocketClient client)
    {
        try
        {
            await client.CreateGlobalApplicationCommandAsync(Command.EventCreate.Create());
            await client.CreateGlobalApplicationCommandAsync(Command.EventRemove.Create());
            await client.CreateGlobalApplicationCommandAsync(Command.EventInfo.Create());
            await client.CreateGlobalApplicationCommandAsync(Command.EventList.Create());
            await client.CreateGlobalApplicationCommandAsync(Command.EventExport.Create());
            await client.CreateGlobalApplicationCommandAsync(Command.Ping.Create());

            Logger.Write($"Registered global slash commands");
        }
        catch (Exception exception)
        {
            Logger.Write($"Failed to register commands: {exception}");
        }
    }
}