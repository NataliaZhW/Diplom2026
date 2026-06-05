using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Models.Tasks;     // Для модели AssignedTask
using BackendWinder.Models.Materials;  // Для модели MaterialRequest
using Microsoft.AspNetCore.Identity;

namespace BackendWinder.Data
{
    // AppDbContext - главный контекст для работы с оперативной базой данных (AppDb)
    // Наследуется от IdentityDbContext, чтобы автоматически создать таблицы пользователей
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Конструктор, принимающий параметры подключения
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Таблица заданий мотальщиков
        public DbSet<AssignedTask> AssignedTasks { get; set; }

        // Таблица запросов материалов
        public DbSet<MaterialRequest> MaterialRequests { get; set; }

        // Таблица профилей пользователей (доп. информация)
        public DbSet<UserProfile> UserProfiles { get; set; }

        // Дополнительная настройка модели (если нужна)
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Вызов базовой настройки Identity
            base.OnModelCreating(builder);

            // Переименовываем таблицы Identity для удобства
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");  // Таблица пользователей
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles");  // Таблица ролей
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles"); // Таблица связи пользователей с ролями
            });

            // Настройка уникального индекса для UserId в UserProfiles
            builder.Entity<UserProfile>()
                .HasIndex(u => u.UserId)
                .IsUnique();
        }
    }
}