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

    // Настройка моделей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Указываем, что это только для чтения
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<ColorThread>().ToTable("ColorThreads");
    }
}