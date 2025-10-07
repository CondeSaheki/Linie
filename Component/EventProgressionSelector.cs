using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public class EventProgressionSelector : IComponent
{
    public static string Name() => "event_progression_selector";

    public static MessageComponent Create(string[] args)
    {
        (string, string)[] options =
        [
            // ("P1: Looper", "P1: Looper"),
            // ("P1: Pantokrator", "P1: Pantokrator"),
            // ("P2: Party Synergy", "P2: Party Synergy"),
            // ("P2: Limitless Synergy", "P2: Limitless Synergy"),
            // ("P3: Intermission", "P3: Intermission"),
            ("P3: Hello, World!", "p3: Hello, World!"),
            ("P3: Monitors", "p3: Monitors"),
            ("P4", "p4"),
            ("P5: Delta", "p5: Delta"),
            ("P5: Sigma", "p5: Sigma"),
            ("P5: Omega", "p5: Omega"),
            ("P6: Cosmo Dive 1", "p6: Cosmo Dive 1"),
            ("P6: Wave Cannon 2+", "p6: Wave Cannon 2+"),
            ("Cleared", "Cleared"),
        ];

        var menu = new SelectMenuBuilder()
            .WithCustomId(string.Join('|', [Name(), .. args]))
            .WithPlaceholder("Progression point")
            .WithMinValues(1)
            .WithMaxValues(1);

        foreach (var (label, value) in options)
        {
            menu.AddOption(label, value);
        }

        return new ComponentBuilder().WithSelectMenu(menu).Build();
    }

    public static async Task HandleAsync(DiscordSocketClient client, SocketMessageComponent component)
    {
        await component.DeferAsync(ephemeral: true);

        var id = component.Data.CustomId.Split('|')[1];

        var @event = await EventManager.GetEventAsync(id) ?? throw new Exception($"Event {id} not found.");
        if (@event.Open == false) throw new Exception($"Event {id} is not open.");
        if (!@event.Participants.TryGetValue(component.User.Id, out var participant))
        {
            participant = new(client, component);
            @event.Participants[component.User.Id] = participant;
        }

        var progression = component.Data.Values?.FirstOrDefault() ?? "";

        if (participant.Progression == progression) return;

        participant.Progression = progression;

        if (participant.Joined)
        {
            participant.Joined = false;
            await component.FollowupAsync("Updating your `Progression` has canceled your participation, you can rejoin at any time", ephemeral: true);
        }

        await EventManager.SaveAsync();
    }
}