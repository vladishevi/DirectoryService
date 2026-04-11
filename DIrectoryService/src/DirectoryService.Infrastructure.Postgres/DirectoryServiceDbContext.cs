using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres;

public class DirectoryServiceDbContext : DbContext
{
    public DirectoryServiceDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Location> Locations => Set<Location>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
    }
}