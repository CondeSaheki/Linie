
using Discord;
using Discord.WebSocket;

namespace Linie.Command;

public class EventCreate : ICommand
{
    public static string Name() => "event_create";

    public static SlashCommandProperties Create() => new SlashCommandBuilder()
        .WithName(Name())
        .WithDescription("Create an event")
        .AddOption("title", ApplicationCommandOptionType.String, "Title", isRequired: true)
        .AddOption("description", ApplicationCommandOptionType.String, "Description", isRequired: false)
        .Build();

    public static async Task HandleAsync(DiscordSocketClient client, SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);

        var title = (string)command.Data.Options.First().Value!;
        var description = (string?)command.Data.Options.ElementAtOrDefault(1)?.Value ?? string.Empty;

        var id = Guid.NewGuid().ToString().Split('-')[0];

        var messages = new (string prompt, MessageComponent? component)[]
        {
            ($"# {title}\n{description}", null),
            ("### What Jobs do you play?", Component.EventJobSelector.Create([id])),
            ("### When are you available?", Component.EventAvailabilitySelector.Create([id])),
            ("### What is your Progression Point?", Component.EventProgressionSelector.Create([id])),
            ("### Want to be a helper?", Component.EventHelperSelector.Create([id])),
            ("### Additional Comments?\nExamples: \"I can fake melee\", \"I only OT\", \"Put me with Nimia\", \"Only one party\".",
                Component.EventCommentButton.Create([id])),
            ("# Confirmation", Component.EventParticipationButtons.Create([id]))
        };

        List<ulong> sent = new(messages.Length);

        foreach (var (prompt, component) in messages)
        {
            var message = component != null ? await command.Channel.SendMessageAsync(prompt, components: component) :
                await command.Channel.SendMessageAsync(prompt);
            sent.Add(message.Id);
        }

        await EventManager.SaveAsync(id,
            new Event
            {
                Title = title,
                Description = description,
                Channel = command.Channel.Id,
                Messages = sent,
                Participants = [],
                Open = true
            }
        );

        await command.FollowupAsync($"Event created with id **{id}**.", ephemeral: true);
    }
}