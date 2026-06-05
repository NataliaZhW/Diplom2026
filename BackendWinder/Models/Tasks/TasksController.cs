using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Data;
using BackendWinder.Models.Tasks;
using System.Security.Claims;

namespace BackendWinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Все эндпоинты требуют авторизации
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ReferenceDbContext _referenceDbContext;

        public TasksController(AppDbContext appDbContext, ReferenceDbContext referenceDbContext)
        {
            _appDbContext = appDbContext;
            _referenceDbContext = referenceDbContext;
        }

        // Получить ID текущего пользователя из токена
        private string GetCurrentUserId()
        {
            return User.FindFirst("userId")?.Value ?? throw new Exception("User not found");
        }

        // ================================================================
        // 1. ПОЛУЧИТЬ МОИ ЗАДАНИЯ
        // GET /api/tasks/my
        // ================================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = GetCurrentUserId();
            
            var tasks = await _appDbContext.AssignedTasks
                .Where(t => t.AssignedToUserId == userId)
                .OrderByDescending(t => t.AssignedAt)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    PlannedTaskId = t.PlannedTaskId,
                    AssignedToUserId = t.AssignedToUserId,
                    TaskType = t.TaskType,
                    KitSchemeCode = t.KitSchemeCode,
                    KitSchemeName = t.KitSchemeName,
                    BrandCode = t.BrandCode,
                    ColorCode = t.ColorCode,
                    CountCode = t.CountCode,
                    Quantity = t.Quantity,
                    Notes = t.Notes,
                    Status = t.Status,
                    AssignedAt = t.AssignedAt,
                    MaterialsRequestedAt = t.MaterialsRequestedAt,
                    MaterialsReceivedAt = t.MaterialsReceivedAt,
                    SubmittedAt = t.SubmittedAt,
                    ReportedAt = t.ReportedAt,
                    ArchivedAt = t.ArchivedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // ================================================================
        // 2. ПОЛУЧИТЬ ЗАДАНИЕ ПО ID
        // GET /api/tasks/{id}
        // ================================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            
            var task = await _appDbContext.AssignedTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(new { message = "Задание не найдено" });

            // Проверка прав: только владелец или админ
            var isAdmin = User.IsInRole("Admin");
            if (task.AssignedToUserId != userId && !isAdmin)
                return Forbid();

            return Ok(task);
        }

        // ================================================================
        // 3. СОЗДАТЬ НОВОЕ ЗАДАНИЕ (ручное)
        // POST /api/tasks
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            var userId = GetCurrentUserId();
            
            var task = new AssignedTask
            {
                AssignedToUserId = userId,
                TaskType = request.TaskType,
                KitSchemeCode = request.KitSchemeCode,
                BrandCode = request.BrandCode,
                ColorCode = request.ColorCode,
                CountCode = request.CountCode,
                Quantity = request.Quantity,
                Notes = request.Notes,
                Status = "New",
                AssignedAt = DateTime.Now,
                PlannedTaskId = null  // Ручное задание, не из плана
            };

            // Если тип Scheme, нужно подтянуть название комплекта
            if (request.TaskType == "Scheme" && !string.IsNullOrEmpty(request.KitSchemeCode))
            {
                var kitScheme = await _referenceDbContext.KitsSchemes
                    .FirstOrDefaultAsync(ks => ks.Code == request.KitSchemeCode);
                if (kitScheme != null)
                    task.KitSchemeName = kitScheme.Name;
            }

            _appDbContext.AssignedTasks.Add(task);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { id = task.Id, message = "Задание создано" });
        }

        // ================================================================
        // 4. ИЗМЕНИТЬ СТАТУС ЗАДАНИЯ
        // PUT /api/tasks/{id}/status
        // ================================================================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusRequest request)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            
            var task = await _appDbContext.AssignedTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(new { message = "Задание не найдено" });

            // Проверка прав
            if (task.AssignedToUserId != userId && !isAdmin)
                return Forbid();

            // Обновляем статус и соответствующую дату
            task.Status = request.NewStatus;
            
            switch (request.NewStatus)
            {
                case "MaterialsRequested":
                    task.MaterialsRequestedAt = DateTime.Now;
                    break;
                case "MaterialsReceived":
                    task.MaterialsReceivedAt = DateTime.Now;
                    break;
                case "InProgress":
                    // Старт работы
                    break;
                case "Submitted":
                    task.SubmittedAt = DateTime.Now;
                    break;
                case "Reported":
                    task.ReportedAt = DateTime.Now;
                    break;
                case "Archived":
                    task.ArchivedAt = DateTime.Now;
                    break;
            }

            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = $"Статус изменен на {request.NewStatus}" });
        }

        // ================================================================
        // 5. ЗАПРОСИТЬ МАТЕРИАЛЫ ДЛЯ ЗАДАНИЯ
        // POST /api/tasks/{id}/request-materials
        // ================================================================
        [HttpPost("{id}/request-materials")]
        public async Task<IActionResult> RequestMaterials(int id, [FromBody] string? notes)
        {
            var userId = GetCurrentUserId();
            
            var task = await _appDbContext.AssignedTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(new { message = "Задание не найдено" });

            if (task.AssignedToUserId != userId)
                return Forbid();

            // Обновляем статус
            task.Status = "MaterialsRequested";
            task.MaterialsRequestedAt = DateTime.Now;

            await _appDbContext.SaveChangesAsync();

            return Ok(new { message = "Запрос материалов отправлен" });
        }
    }
}