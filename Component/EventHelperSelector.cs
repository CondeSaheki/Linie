using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public class EventHelperSelector : IComponent
{
    public static string Name() => "event_helper_selector";

    public static MessageComponent Create(string[] args)
    {
        (string, string)[] options =
        [
            ("Yes", "true"),
            ("No", "false")
        ];

        var menu = new SelectMenuBuilder()
            .WithCustomId(string.Join('|', [Name(), .. args]))
            .WithPlaceholder("Want to be a helper?")
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

        var helper = component.Data.Values?.FirstOrDefault() == "true";

        if (participant.Helper == helper) return;

        participant.Helper = helper;

        if (participant.Joined)
        {
            participant.Joined = false;
            await component.FollowupAsync("Updating your `helper` has canceled your participation, you can rejoin at any time", ephemeral: true);
        }

        await EventManager.SaveAsync();
    }
}