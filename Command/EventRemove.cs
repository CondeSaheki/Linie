using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public class EventRemove : ICommand
{
    public static string Name() => "event_remove";

    public static SlashCommandProperties Create() => new SlashCommandBuilder()
        .WithName(Name())
        .WithDescription("Close an event and remove its messages")
        .AddOption("id", ApplicationCommandOptionType.String, "Event Id", isRequired: true)
        .Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);

        var id = (string)command.Data.Options.First().Value!;
        var @event = await EventManager.GetEventAsync(id);

        if (@event == null)
        {
            await command.FollowupAsync($"Event id `{id}` not found", ephemeral: true);
            return;
        }
        if (!@event.Open)
        {
            await command.FollowupAsync($"Event id `{id}` is already closed.", ephemeral: true);
            return;
        }

        @event.Open = false;

        if (client.GetChannel(@event.Channel) is IMessageChannel channel)
        {
            foreach (var messageId in @event.Messages)
            {
                try
                {
                    var message = await channel.GetMessageAsync(messageId);
                    if (message != null) await message.DeleteAsync();
                }
                catch { /* ignore */ }
            }
            @event.Messages = [];
            @event.Channel = 0;
        }

        await EventManager.SaveAsync();
        await command.FollowupAsync($"Event `{id}` closed.", ephemeral: true);
    }
}