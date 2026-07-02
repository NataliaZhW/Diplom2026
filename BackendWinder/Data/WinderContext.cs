using BackendWinder.Models.Winder;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Data;

/// <summary>
/// Контекст базы данных для db-winder (оперативные данные, полный доступ)
/// </summary>
public class WinderContext : DbContext
{
    public WinderContext(DbContextOptions<WinderContext> options)
        : base(options)
    {
    }

    public DbSet<BrandManuf> BrandManufs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BrandManuf>().ToTable("BrandManufs");
    }
}