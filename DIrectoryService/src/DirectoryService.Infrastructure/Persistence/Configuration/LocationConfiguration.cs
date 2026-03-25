using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Persistence;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id).HasName("id");
        
        builder.Property(l => l.Name)
            .HasConversion(l => l.Value, l => Name.Create(l).Value)
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGHT);

        builder.OwnsOne(l => l.Address, ab =>
        {
            ab.Property(a => a.City)
                .HasColumnName("city")
                .IsRequired();
            ab.Property(a => a.Street)
                .HasColumnName("street")
                .IsRequired();
            ab.Property(a => a.Building)
                .HasColumnName("building")
                .IsRequired();
            ab.Property(a => a.Postcode)
                .HasColumnName("postcode")
                .IsRequired();
        });

        builder.Property(l => l.Timezone)
            .HasConversion(l => l.Code, l => Timezone.Create(l).Value)
            .IsRequired();
    }
}