namespace Percolator.Domain;

public class Invite
{
    public Guid Id { get; private set; }
    public Guid CatchupId { get; private set; }
    public Guid PersonId { get; private set; }
    public RsvpStatus RsvpStatus { get; private set; }

    private Invite()
    {
        // EF Core
    }

    internal Invite(Guid catchupId, Guid personId)
    {
        Id = Guid.NewGuid();
        CatchupId = catchupId;
        PersonId = personId;
        RsvpStatus = RsvpStatus.Pending;
    }

    public void Respond(RsvpStatus status) => RsvpStatus = status;
}
