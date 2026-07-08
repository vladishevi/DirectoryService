using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Infrastructure.Postgres.Features.Departments;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        builder.HasKey(d => d.Id).HasName("pk_departments");

        builder.Property(d => d.Id).
            HasColumnName("id");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasColumnType("citext")
            .HasConversion(name => name.Value, value => Name.Create(value).Value)
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGHT);
        
        builder.Property(d => d.Identifier)
            .HasColumnName("identifier")
            .HasColumnType("citext")
            .HasConversion(identifier => identifier.Value, d => Identifier.Create(d).Value)
            .IsRequired()
            .HasMaxLength(Identifier.MAX_LENGHT);
        
        builder.Property(d => d.Path)
            .HasColumnName("path")
            .HasColumnType("ltree")
            .HasConversion(p => p.Value, p => new Path(p));
        
        builder.Property(d => d.Depth)
            .HasColumnName("depth")
            .IsRequired();
        
        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(d => d.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();
        
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(d => d.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasOne(d => d.ParentDepartment)
            .WithMany()
            .HasForeignKey("parent_department_id")
            .IsRequired(false);
        
        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasDatabaseName(Constants.Indexes.DEPARTMENT_NAME);

        builder.HasIndex(d => d.Identifier)
            .IsUnique()
            .HasDatabaseName(Constants.Indexes.DEPARTMENT_IDENTIFIER);
        
        builder.HasIndex(d => d.Path)
            .HasMethod("gist")
            .HasDatabaseName(Constants.Indexes.DEPARTMENT_PATH);
    }
}
