using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Percolator.Domain;
using Percolator.Persistence.EntityFrameworkCore;

namespace Percolator.Persistence.Tests;

public class PercolatorDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly PercolatorDbContext _context;

    public PercolatorDbContextTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _context = CreateContext();
        _context.Database.EnsureCreated();
    }

    private PercolatorDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PercolatorDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new PercolatorDbContext(options);
    }

    [Fact]
    public void PercolatorDbContext_exposes_no_DbSet_properties()
    {
        var dbSetProperties = typeof(PercolatorDbContext)
            .GetProperties()
            .Where(property =>
                property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        Assert.Empty(dbSetProperties);
    }

    [Fact]
    public void Catchup_and_its_invites_round_trip_through_Set()
    {
        var cafe = new Cafe("Common Grounds", "12 Bean St");
        var alex = new Person("Alex", "alex@example.com", usualOrder: "Almond Flat white");
        var sam = new Person("Sam", "sam@example.com");

        _context.Set<Cafe>().Add(cafe);
        _context.Set<Person>().AddRange(alex, sam);
        _context.SaveChanges();

        var catchup = new Catchup("Monthly sync", DateTimeOffset.UtcNow.AddDays(7), cafe.Id);
        catchup.InvitePerson(alex.Id);
        catchup.InvitePerson(sam.Id);

        _context.Set<Catchup>().Add(catchup);
        _context.SaveChanges();

        using var verifyContext = CreateContext();

        var reloaded = verifyContext.Set<Catchup>()
            .Include(c => c.Invites)
            .Single(c => c.Id == catchup.Id);

        Assert.Equal(CatchupStatus.Proposed, reloaded.Status);
        Assert.Equal(cafe.Id, reloaded.CafeId);
        Assert.Equal(2, reloaded.Invites.Count);
        Assert.All(reloaded.Invites, invite => Assert.Equal(RsvpStatus.Pending, invite.RsvpStatus));
    }

    [Fact]
    public void Deleting_a_catchup_cascades_to_its_invites()
    {
        var person = new Person("Jamie", "jamie@example.com");
        _context.Set<Person>().Add(person);
        _context.SaveChanges();

        var catchup = new Catchup("Quick chat", DateTimeOffset.UtcNow.AddDays(1));
        var invite = catchup.InvitePerson(person.Id);
        _context.Set<Catchup>().Add(catchup);
        _context.SaveChanges();

        _context.Set<Catchup>().Remove(catchup);
        _context.SaveChanges();

        Assert.Null(_context.Set<Invite>().Find(invite.Id));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
