using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Percolator.Domain;

namespace Percolator.Persistence.EntityFrameworkCore.EntityConfigurations;

public class InviteConfiguration : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.ToTable("Invites");

        builder.HasKey(invite => invite.Id);

        builder.Property(invite => invite.RsvpStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne<Person>()
            .WithMany()
            .HasForeignKey(invite => invite.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
