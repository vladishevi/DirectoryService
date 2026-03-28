using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Persistence;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");
        builder.HasKey(p => p.Id).HasName("pk_positions");

        builder.Property(p => p.Id).HasColumnName("id");
        
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasConversion(p => p.Value, p => Name.Create(p).Value)
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGHT);

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasConversion(
                p => p != null ? p.Value : null,
                p => p != null ? Description.Create(p).Value : null)
            .IsRequired(false)
            .HasMaxLength(Description.MAX_LENGTH);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
