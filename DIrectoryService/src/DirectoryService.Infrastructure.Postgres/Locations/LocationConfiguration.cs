using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id).HasName("pk_locations");
        
        builder.Property(l => l.Id).HasColumnName("id");

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .HasColumnType("citext")
            .HasConversion(l => l.Value, l => Name.Create(l).Value)
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGHT);

        builder.OwnsOne(l => l.Address, a =>
        {
            a.Property(a => a.City)
                .HasColumnName("city")
                .HasColumnType("citext")
                .IsRequired();
            a.Property(a => a.Street)
                .HasColumnName("street")
                .HasColumnType("citext")
                .IsRequired();
            a.Property(a => a.Building)
                .HasColumnName("building")
                .HasColumnType("citext")
                .IsRequired();
            a.Property(a => a.Postcode)
                .HasColumnName("postcode")
                .HasColumnType("citext")
                .IsRequired();

            a.HasIndex(a => new { a.City, a.Street, a.Building, a.Postcode })
                .IsUnique()
                .HasDatabaseName(Constants.Indexes.LOCATION_ADDRESS);
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
            .ValueGeneratedOnUpdate()
            .IsRequired();

        builder.HasIndex(l => l.Name)
            .IsUnique()
            .HasDatabaseName(Constants.Indexes.LOCATION_NAME);
    }
}