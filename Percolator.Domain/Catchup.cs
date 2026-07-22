namespace Percolator.Domain;

public class Catchup
{
    private readonly List<Invite> _invites = new();

    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public DateTimeOffset ScheduledFor { get; private set; }
    public CatchupStatus Status { get; private set; }
    public Guid? CafeId { get; private set; }

    public IReadOnlyCollection<Invite> Invites => _invites.AsReadOnly();

    private Catchup()
    {
        // EF Core
    }

    public Catchup(string title, DateTimeOffset scheduledFor, Guid? cafeId = null)
    {
        Id = Guid.NewGuid();
        Title = title;
        ScheduledFor = scheduledFor;
        CafeId = cafeId;
        Status = CatchupStatus.Proposed;
    }

    public Invite InvitePerson(Guid personId)
    {
        var invite = new Invite(Id, personId);
        _invites.Add(invite);
        return invite;
    }

    public void Confirm() => Status = CatchupStatus.Confirmed;

    public void Complete() => Status = CatchupStatus.Completed;

    public void Cancel() => Status = CatchupStatus.Cancelled;
}
