using Discord;
using Discord.WebSocket;

namespace Linie.Component;

public class EventJobSelector : IComponent
{
    public static string Name() => "event_job_selector";

    public static MessageComponent Create(string[] args)
    {
        var menu = new SelectMenuBuilder()
            .WithCustomId(string.Join('|', [Name(), .. args]))
            .WithPlaceholder("Choose your job(s)")
            .WithMinValues(1)
            .WithMaxValues(FFXIV.Jobs.Length);

        foreach (var job in FFXIV.Jobs)
        {
            menu.AddOption(FFXIV.JobName(job), FFXIV.JobAbreviation(job));
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

        var jobs = component.Data.Values?.ToList() ?? [];

        if (participant.Jobs == jobs) return;

        participant.Jobs = jobs;

        if (participant.Joined)
        {
            participant.Joined = false;
            await component.FollowupAsync("Updating your `Jobs` has canceled your participation, you can rejoin at any time", ephemeral: true);
        }

        await EventManager.SaveAsync();
    }
}
