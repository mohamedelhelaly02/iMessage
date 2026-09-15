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

        builder.Property(x => x.Title).IsRequired(false);

        builder.Property(x => x.ConversationType)
            .HasConversion(
            x => x.ToString(),
            x => Enum.Parse<ConversationType>(x)
            );


        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.Conversations)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Participants)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Conversations");
    }
}
