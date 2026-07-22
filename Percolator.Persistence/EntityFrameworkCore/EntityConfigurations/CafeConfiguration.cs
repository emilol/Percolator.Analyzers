using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Percolator.Domain;

namespace Percolator.Persistence.EntityFrameworkCore.EntityConfigurations;

public class CafeConfiguration : IEntityTypeConfiguration<Cafe>
{
    public void Configure(EntityTypeBuilder<Cafe> builder)
    {
        builder.ToTable("Cafes");

        builder.HasKey(cafe => cafe.Id);

        builder.Property(cafe => cafe.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cafe => cafe.Address)
            .IsRequired()
            .HasMaxLength(400);
    }
}
