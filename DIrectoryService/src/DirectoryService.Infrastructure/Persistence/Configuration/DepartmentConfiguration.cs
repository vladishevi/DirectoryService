using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Infrastructure.Persistence;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        builder.HasKey(d => d.Id).HasName("pk_departments");

        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasConversion(d => d.Value, d => Name.Create(d).Value)
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGHT);
        
        builder.Property(d => d.Identifier)
            .HasColumnName("identifier")
            .HasConversion(d => d.Value, d => Identifier.Create(d).Value)
            .IsRequired()
            .HasMaxLength(Identifier.MAX_LENGHT);
        
        builder.Property(d => d.Path)
            .HasColumnName("path")
            .HasConversion(p => p.Value, p => new Path(p));
        
        builder.Property(d => d.Depth)
            .HasColumnName("depth")
            .IsRequired();
        
        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
        
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}