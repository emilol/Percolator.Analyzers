using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Percolator.Domain;

namespace Percolator.Persistence.EntityFrameworkCore.EntityConfigurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");

        builder.HasKey(person => person.Id);

        builder.Property(person => person.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(person => person.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(person => person.UsualOrder)
            .HasMaxLength(200);

        builder.HasIndex(person => person.Email)
            .IsUnique();
    }
}
