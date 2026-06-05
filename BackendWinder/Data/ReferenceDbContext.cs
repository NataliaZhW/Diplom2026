using Microsoft.EntityFrameworkCore;
using BackendWinder.Models.Reference;

namespace BackendWinder.Data
{
    // ReferenceDbContext - контекст для работы со справочной базой данных (ReferenceDb)
    // Используется ТОЛЬКО для чтения (SELECT)
    public class ReferenceDbContext : DbContext
    {
        // Конструктор
        public ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : base(options)
        {
        }

        // Таблица брендов
        public DbSet<BrandManuf> BrandManufs { get; set; }

        // Таблица цветов
        public DbSet<ColorThread> ColorThreads { get; set; }

        // Таблица каунтов
        public DbSet<CountType> CountTypes { get; set; }

        // Таблица комплектов
        public DbSet<KitScheme> KitsSchemes { get; set; }

        // Таблица состава комплектов
        public DbSet<KitSchemeComposition> KitSchemeCompositions { get; set; }

        // Таблица вариантов схем
        public DbSet<SchemeVariant> SchemeVariants { get; set; }

        // Таблица деталей схем (количество пасм)
        public DbSet<SchemeVariantDetail> SchemeVariantDetails { get; set; }

        // Таблица вариантов наборов
        public DbSet<KitVariant> KitVariants { get; set; }

        // Таблица деталей наборов (метраж)
        public DbSet<KitVariantDetail> KitVariantDetails { get; set; }

        // Настройка моделей
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка внешних ключей для составов комплектов
            modelBuilder.Entity<KitSchemeComposition>()
                .HasOne(k => k.KitScheme)
                .WithMany()
                .HasForeignKey(k => k.KitSchemeId);

            modelBuilder.Entity<KitSchemeComposition>()
                .HasOne(k => k.RegularThread)
                .WithMany()
                .HasForeignKey(k => k.RegularThreadId);

            modelBuilder.Entity<KitSchemeComposition>()
                .HasOne(k => k.PerleThread)
                .WithMany()
                .HasForeignKey(k => k.PerleThreadId);

            modelBuilder.Entity<KitSchemeComposition>()
                .HasOne(k => k.MetallicThread)
                .WithMany()
                .HasForeignKey(k => k.MetallicThreadId);

            modelBuilder.Entity<KitSchemeComposition>()
                .HasOne(k => k.BrandManuf)
                .WithMany()
                .HasForeignKey(k => k.BrandManufId);
        }
    }
}