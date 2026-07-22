using Microsoft.EntityFrameworkCore;

namespace Percolator.Persistence.EntityFrameworkCore;

public class PercolatorDbContext(DbContextOptions<PercolatorDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PercolatorDbContext).Assembly);
    }
}
