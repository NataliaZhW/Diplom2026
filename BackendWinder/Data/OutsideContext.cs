using BackendWinder.Models.Outside;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Data;

/// <summary>
/// Контекст базы данных для db-outside (справочники, только чтение)
/// </summary>
public class OutsideContext : DbContext
{
    public OutsideContext(DbContextOptions<OutsideContext> options)
        : base(options)
    {
    }

    // DbSet для каждой таблицы
    public DbSet<User> Users { get; set; }
    public DbSet<ColorThread> ColorThreads { get; set; }
    public DbSet<Icon> Icons { get; set; }
    public DbSet<Kit> Kits { get; set; }
    public DbSet<KitComposition> KitCompositions { get; set; }

    // Настройка моделей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Указываем, что это только для чтения
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<ColorThread>().ToTable("ColorThreads");
        modelBuilder.Entity<Icon>().ToTable("Icons");
        modelBuilder.Entity<Kit>().ToTable("Kits");
        modelBuilder.Entity<KitComposition>().ToTable("KitCompositions");

        // Настройка связей
        modelBuilder.Entity<KitComposition>()
            .HasOne(kc => kc.Kit)
            .WithMany(k => k.Compositions)
            .HasForeignKey(kc => kc.KitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KitComposition>()
            .HasOne(kc => kc.Icon)
            .WithMany()
            .HasForeignKey(kc => kc.IconId);

        modelBuilder.Entity<KitComposition>()
            .HasOne(kc => kc.Color)
            .WithMany()
            .HasForeignKey(kc => kc.ColorId);
    }
}