using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Noter.Domain;

public class Note
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string UserId { get; set; }
    public User User { get; set; } = null!;
}

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.HasIndex(x => x.Title).IsUnique();
        builder.Property(x => x.Content).HasMaxLength(50000);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Notes)
            .HasForeignKey("UserId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}