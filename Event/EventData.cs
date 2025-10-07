namespace Linie;

public class Event
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    private bool open;
    public bool Open
    {
        get => open;
        set
        {
            if (open != value)
            {
                open = value;
                if (open)
                {
                    OpenedDateTime = DateTime.UtcNow;
                    ClosedDateTime = null;
                }
                else
                {
                    ClosedDateTime = DateTime.UtcNow;
                }
            }
        }
    }

    public DateTime? OpenedDateTime { get; set; } = null;
    public DateTime? ClosedDateTime { get; set; } = null;

    public ulong Channel { get; set; } = 0;
    public List<ulong> Messages { get; set; } = [];

    public Dictionary<ulong, EventParticipant> Participants { get; set; } = [];
}