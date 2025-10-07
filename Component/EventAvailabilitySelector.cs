using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public class EventAvailabilitySelector : IComponent
{
    public static string Name() => "event_availability_selector";

    public static MessageComponent Create(string[] args)
    {
        string[] options =
        [
            "12 PM EST",
            "2 PM EST",
            "4 PM EST",
            "6 PM EST",
            "8 PM EST",
            "10 PM EST",
            "12 AM EST"
        ];

        var menu = new SelectMenuBuilder()
            .WithCustomId(string.Join('|', [Name(), .. args]))
            .WithPlaceholder("Choose your availability")
            .WithMinValues(1)
            .WithMaxValues(options.Length);

        foreach (var availability in options)
        {
            menu.AddOption(availability, availability);
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

        var availability = component.Data.Values?.ToList() ?? [];

        if (participant.Availability == availability) return;
        
        participant.Availability = availability;
        
        if (participant.Joined)
        {
            participant.Joined = false;
            await component.FollowupAsync("Updating your `availability` has canceled your participation, you can rejoin at any time", ephemeral: true);
        }

        await EventManager.SaveAsync();
    }
}