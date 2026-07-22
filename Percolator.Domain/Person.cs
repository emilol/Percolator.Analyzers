namespace Percolator.Domain;

public class Person
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? UsualOrder { get; private set; }

    private Person()
    {
        // EF Core
    }

    public Person(string name, string email, string? usualOrder = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        UsualOrder = usualOrder;
    }
}
