using System.Text;
using BackendWinder.Data;
using BackendWinder.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BackendWinder.Services.Interfaces;
using BackendWinder.Filters;
using BackendWinder.Models.Config;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Настройка подключения к базам данных
// ============================================================

// Подключение к db-outside (справочники)
var outsideConnection = builder.Configuration.GetConnectionString("Outside");
builder.Services.AddDbContext<OutsideContext>(options =>
    options.UseMySql(outsideConnection, ServerVersion.AutoDetect(outsideConnection))
);

// Подключение к db-winder (оперативные данные)
var winderConnection = builder.Configuration.GetConnectionString("Winder");
builder.Services.AddDbContext<WinderContext>(options =>
    options.UseMySql(winderConnection, ServerVersion.AutoDetect(winderConnection))
);

// ============================================================
// 2. Настройка JWT авторизации
// ============================================================

var jwtSecret = builder.Configuration["JWT:Secret"] 
    ?? throw new Exception("JWT Secret not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"] 
    ?? throw new Exception("JWT Issuer not configured");
var jwtAudience = builder.Configuration["JWT:Audience"] 
    ?? throw new Exception("JWT Audience not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ============================================================
// 3. Регистрация своих сервисов
// ============================================================

// Регистрация бизнес-правил
var businessRules = builder.Configuration.GetSection("BusinessRules").Get<BusinessRulesConfig>()
    ?? new BusinessRulesConfig();  // ← если null, создаём новый объект

builder.Services.AddSingleton(businessRules);

//builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IReferenceService, ReferenceService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ============================================================
// 4. Настройка CORS (разрешаем запросы с фронтенда)
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================
// 5. Настройка контроллеров и Swagger
// ============================================================

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// Настройка Swagger с поддержкой JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Diplom_Winder API", 
        Version = "v1",
        Description = "API для управления заданиями мотальщиков"
    });

    // Добавляем возможность авторизации в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите JWT токен: Bearer {токен}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================
// 6. Сборка приложения
// ============================================================

var app = builder.Build();

// ============================================================
// 7. Настройка middleware (порядок важен!)
// ============================================================

// Swagger (только в разработке)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Перенаправление на HTTPS
app.UseHttpsRedirection();

// CORS (разрешаем запросы с фронтенда)
app.UseCors("AllowFrontend");

// Аутентификация и авторизация (порядок важен!)
app.UseAuthentication();
app.UseAuthorization();

// Контроллеры
app.MapControllers();

// ============================================================
// 8. Запуск приложения
// ============================================================

app.Run();