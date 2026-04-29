using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");
        
        builder.HasKey(d => d.Id).HasName("pk_department_locations");
        
        builder.Property(d => d.Id).HasColumnName("id");
        
        builder.HasOne(dc => dc.Department)
            .WithMany(d => d.Locations)
            .HasForeignKey(d => d.DepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Location>()
            .WithMany(l => l.Departments)
            .HasForeignKey(l => l.LocationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(dl => dl.LocationId)
            .HasColumnName("location_id");
        
        builder.Property(dl => dl.DepartmentId)
            .HasColumnName("department_id");
    }
}