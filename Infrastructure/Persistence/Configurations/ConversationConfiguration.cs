using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Title).IsRequired();

        builder.Property(x => x.ConversationType)
            .HasConversion(
            x => x.ToString(),
            x => Enum.Parse<ConversationType>(x)
            );


        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.ToTable("Conversation");
    }
}
