namespace Percolator.Domain;

public class Cafe
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;

    private Cafe()
    {
        // EF Core
    }

    public Cafe(string name, string address)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
    }
}
