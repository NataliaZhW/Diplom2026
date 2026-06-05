using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;  
using BackendWinder.Data;
using BackendWinder.Services;

namespace BackendWinder
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================================================
            // 1. НАСТРОЙКА ПОДКЛЮЧЕНИЯ К БАЗАМ ДАННЫХ
            // ================================================

            // Получаем строки подключения из appsettings.json
            var referenceConnectionString = builder.Configuration.GetConnectionString("ReferenceDb");
            var appConnectionString = builder.Configuration.GetConnectionString("AppDb");

            // Регистрируем контекст для справочной БД (только чтение)
            builder.Services.AddDbContext<ReferenceDbContext>(options =>
                options.UseMySql(referenceConnectionString,
                    ServerVersion.AutoDetect(referenceConnectionString)));

            // Регистрируем контекст для оперативной БД (полный доступ)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(appConnectionString,
                    ServerVersion.AutoDetect(appConnectionString)));

            // ================================================
            // 2. НАСТРОЙКА IDENTITY
            // ================================================

            // Добавляем Identity с нашим расширенным пользователем ApplicationUser
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()  // Храним данные пользователей в AppDb
                .AddDefaultTokenProviders();               // Для сброса пароля (если понадобится)

            // ================================================
            // 3. НАСТРОЙКА JWT АУТЕНТИФИКАЦИИ
            // ================================================

            // Получаем настройки JWT из appsettings.json
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key not configured");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer not configured");
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new Exception("JWT Audience not configured");
            var jwtExpiryMinutes = int.Parse(builder.Configuration["Jwt:ExpiryMinutes"] ?? "1440");

            // Преобразуем ключ в байты для подписи
            var key = Encoding.UTF8.GetBytes(jwtKey);

            // Настраиваем аутентификацию через JWT Bearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;  // Для разработки (в продакшене нужно true)
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,           // Проверяем издателя
                    ValidateAudience = true,         // Проверяем получателя
                    ValidateLifetime = true,         // Проверяем срок действия
                    ValidateIssuerSigningKey = true, // Проверяем подпись
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Добавляем авторизацию
            builder.Services.AddAuthorization();

            // ================================================
            // 4. ДОБАВЛЯЕМ КОНТРОЛЛЕРЫ И SWAGGER
            // ================================================

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BackendWinder API", Version = "v1" });

                // Настройка JWT для Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Введите токен в формате: eyJhbGciOiJIUzI1NiIs..."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Регистрация JWT сервиса
            builder.Services.AddScoped<JwtService>();

            // ================================================
            // 5. НАСТРОЙКА CORS (для Vue.js фронтенда)
            // ================================================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueFrontend", policy =>
                {
                    // Разрешаем запросы с localhost:5173 (Vite) и 5174 (альтернативный порт)
                    policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();  // Разрешаем отправку cookies (если понадобятся)
                });
            });

            // ================================================
            // 6. ПОСТРОЕНИЕ ПРИЛОЖЕНИЯ
            // ================================================

            var app = builder.Build();

            // Настройка Swagger в режиме разработки
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Перенаправление HTTP на HTTPS (в разработке можно отключить)
            // app.UseHttpsRedirection();

            // Включаем CORS
            app.UseCors("AllowVueFrontend");

            // Включаем аутентификацию и авторизацию
            app.UseAuthentication();
            app.UseAuthorization();

            // Маппинг контроллеров
            app.MapControllers();

            // ================================================
            // 7. СОЗДАНИЕ БАЗЫ ДАННЫХ ПРИ ПЕРВОМ ЗАПУСКЕ
            // ================================================

            using (var scope = app.Services.CreateScope())
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Создаем базу данных, если её нет
                // Это создаст все таблицы: AspNetUsers, AspNetRoles, AssignedTasks, MaterialRequests, UserProfiles
                appDbContext.Database.EnsureCreated();

                // Создаем роли, если их нет

                string[] roleNames = { "User", "Admin" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }

            // Запуск приложения
            app.Run();
        }
    }
}