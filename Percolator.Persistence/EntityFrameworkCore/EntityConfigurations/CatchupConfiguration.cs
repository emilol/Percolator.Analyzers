using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Percolator.Domain;

namespace Percolator.Persistence.EntityFrameworkCore.EntityConfigurations;

public class CatchupConfiguration : IEntityTypeConfiguration<Catchup>
{
    public void Configure(EntityTypeBuilder<Catchup> builder)
    {
        builder.ToTable("Catchups");

        builder.HasKey(catchup => catchup.Id);

        builder.Property(catchup => catchup.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(catchup => catchup.ScheduledFor)
            .IsRequired();

        builder.Property(catchup => catchup.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne<Cafe>()
            .WithMany()
            .HasForeignKey(catchup => catchup.CafeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(catchup => catchup.Invites)
            .WithOne()
            .HasForeignKey(invite => invite.CatchupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(catchup => catchup.Invites)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
