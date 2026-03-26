using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Persistence;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id).HasName("pk_locations");
        
        builder.Property(l => l.Name)
            .HasColumnName("name")
            .HasConversion(l => l.Value, l => Name.Create(l).Value)
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGHT);

        builder.OwnsOne(l => l.Address, ab =>
        {
            ab.ToJson("address");
            
            ab.Property(a => a.City)
                .IsRequired();
            ab.Property(a => a.Street)
                .IsRequired();
            ab.Property(a => a.Building)
                .IsRequired();
            ab.Property(a => a.Postcode)
                .IsRequired();
        });

        builder.Property(l => l.Timezone)
            .HasColumnName("timezone")
            .HasConversion(l => l.Code, l => Timezone.Create(l).Value)
            .IsRequired();
        
        builder.Property(l => l.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
        
        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}