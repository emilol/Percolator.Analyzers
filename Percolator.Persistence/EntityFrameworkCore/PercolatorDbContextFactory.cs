using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Percolator.Persistence.EntityFrameworkCore;

/// <summary>
/// Lets "dotnet ef migrations add" construct a context at design time, since this
/// project has no Program.cs/host to resolve DbContextOptions from.
/// </summary>
public class PercolatorDbContextFactory : IDesignTimeDbContextFactory<PercolatorDbContext>
{
    public PercolatorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PercolatorDbContext>()
            .UseSqlite("Data Source=percolator.db");

        return new PercolatorDbContext(optionsBuilder.Options);
    }
}
