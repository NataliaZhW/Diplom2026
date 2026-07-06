using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Models.Winder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly WinderContext _winderContext;
    private readonly OutsideContext _outsideContext;
    private readonly ILogger<TasksController> _logger;

    private static readonly Dictionary<string, string> StatusLabels = new()
    {
        { "new", "Новое" },
        { "planned", "Запланировано" },
        { "materials_requested", "Материалы запрошены" },
        { "materials_issued", "Материалы выданы" },
        { "in_progress", "В работе" },
        { "completed", "Сдано" },
        { "accepted", "Принято" },
        { "archived", "В архиве" }
    };

    private static readonly Dictionary<string, string> TypeLabels = new()
    {
        { "kit", "Набор" },
        { "scheme", "Схема" },
        { "thread", "Нить" }
    };

    public TasksController(
        WinderContext winderContext,
        OutsideContext outsideContext,
        ILogger<TasksController> logger)
    {
        _winderContext = winderContext;
        _outsideContext = outsideContext;
        _logger = logger;
    }

    // ============================================================
    // 1. Получить все задания
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdClaim?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";

        var query = _winderContext.Tasks.AsQueryable();

        if (userRole != "master")
        {
            query = query.Where(t => t.WinderId == userId);
        }

        var tasksFromDb = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var winderIds = tasksFromDb.Select(t => t.WinderId).Distinct().ToList();

        var userNames = new Dictionary<int, string>();
        if (winderIds.Any())
        {
            var users = await _outsideContext.Users
                .Where(u => winderIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            foreach (var user in users)
            {
                userNames[user.Id] = user.FullName;
            }
        }

        var result = tasksFromDb.Select(t => new TaskDto
        {
            Id = t.Id,
            Status = t.Status,
            StatusLabel = StatusLabels.GetValueOrDefault(t.Status, t.Status),
            ItemType = t.ItemType,
            ItemTypeLabel = TypeLabels.GetValueOrDefault(t.ItemType, t.ItemType),
            ItemId = t.ItemId,
            ItemCode = t.ItemCode,
            ItemName = t.ItemName,
            BrandLabel = t.BrandLabel,
            CountValue = t.CountValue,
            Quantity = t.Quantity,
            WinderId = t.WinderId,
            WinderName = userNames.GetValueOrDefault(t.WinderId, "Неизвестно"),
            CreatedAt = t.CreatedAt,
            AssignedAt = t.AssignedAt,
            MaterialsIssuedAt = t.MaterialsIssuedAt,
            CompletedAt = t.CompletedAt,
            AcceptedAt = t.AcceptedAt,
            ArchivedAt = t.ArchivedAt,
            Note = t.Note
        }).ToList();

        return Ok(result);
    }

    // ============================================================
    // 2. Получить задание по ID
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _winderContext.Tasks
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();

        if (task == null)
            return NotFound(new { message = "Задание не найдено" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";

        if (userRole != "master" && task.WinderId != userId)
            return Forbid();

        var winderName = "Неизвестно";
        var user = await _outsideContext.Users
            .Where(u => u.Id == task.WinderId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync();

        if (user != null)
            winderName = user;

        var result = new TaskDto
        {
            Id = task.Id,
            Status = task.Status,
            StatusLabel = StatusLabels.GetValueOrDefault(task.Status, task.Status),
            ItemType = task.ItemType,
            ItemTypeLabel = TypeLabels.GetValueOrDefault(task.ItemType, task.ItemType),
            ItemId = task.ItemId,
            ItemCode = task.ItemCode,
            ItemName = task.ItemName,
            BrandLabel = task.BrandLabel,
            CountValue = task.CountValue,
            Quantity = task.Quantity,
            WinderId = task.WinderId,
            WinderName = winderName,
            CreatedAt = task.CreatedAt,
            AssignedAt = task.AssignedAt,
            MaterialsIssuedAt = task.MaterialsIssuedAt,
            CompletedAt = task.CompletedAt,
            AcceptedAt = task.AcceptedAt,
            ArchivedAt = task.ArchivedAt,
            Note = task.Note
        };

        return Ok(result);
    }

    // ============================================================
    // 3. Создать задание
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";

        if (userRole != "master")
            return Forbid();

        var task = new WinderTask
        {
            Status = "new",
            ItemType = dto.ItemType,
            ItemId = dto.ItemId,
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            BrandLabel = dto.BrandLabel,
            CountValue = dto.CountValue,
            Quantity = dto.Quantity,
            WinderId = dto.WinderId,
            AssignedBy = userId,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Note = dto.Note
        };

        _winderContext.Tasks.Add(task);
        await _winderContext.SaveChangesAsync();

        _logger.LogInformation($"Создано задание {task.Id} для пользователя {task.WinderId}");

        return Ok(new { id = task.Id, message = "Задание создано" });
    }

    // ============================================================
    // 4. Обновить статус задания
    // ============================================================
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskStatusUpdateDto dto)
    {
        var task = await _winderContext.Tasks.FindAsync(id);
        if (task == null)
            return NotFound(new { message = "Задание не найдено" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";

        if (userRole != "master" && task.WinderId != userId)
            return Forbid();

        task.Status = dto.Status;

        switch (dto.Status)
        {
            case "planned":
                task.AssignedAt = DateTime.UtcNow;
                break;
            case "materials_issued":
                task.MaterialsIssuedAt = DateTime.UtcNow;
                break;
            case "completed":
                task.CompletedAt = DateTime.UtcNow;
                break;
            case "accepted":
                task.AcceptedAt = DateTime.UtcNow;
                break;
            case "archived":
                task.ArchivedAt = DateTime.UtcNow;
                break;
        }

        await _winderContext.SaveChangesAsync();

        _logger.LogInformation($"Обновлён статус задания {id} на {dto.Status}");

        return Ok(new { message = "Статус обновлён" });
    }

    // ============================================================
    // 5. Удалить задание (только new)
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _winderContext.Tasks.FindAsync(id);
        if (task == null)
            return NotFound(new { message = "Задание не найдено" });

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";
        if (userRole != "master")
            return Forbid();

        if (task.Status != "new")
            return BadRequest(new { message = "Можно удалить только задания в статусе 'Новое'" });

        _winderContext.Tasks.Remove(task);
        await _winderContext.SaveChangesAsync();

        _logger.LogInformation($"Удалено задание {id}");

        return Ok(new { message = "Задание удалено" });
    }

    // ============================================================
    // 6. Массовое создание заданий из списка "Выбрано"
    // ============================================================
    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatchTasks([FromBody] List<TaskCreateDto> items)
    {
        if (items == null || items.Count == 0)
            return BadRequest(new { message = "Список заданий пуст" });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";

        // Проверка прав
        if (userRole != "master")
        {
            var hasOtherWinder = items.Any(item => item.WinderId != userId);
            if (hasOtherWinder)
            {
                _logger.LogWarning($"Мотальщик {userId} попытался создать задание для другого пользователя");
                return Forbid();
            }
        }

        // Валидация каждого элемента
        foreach (var dto in items)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                return BadRequest(new { errors = validationResults.Select(v => v.ErrorMessage) });
            }
        }

        var createdTasks = new List<int>();

        foreach (var dto in items)
        {
            var task = new WinderTask
            {
                Status = "new",
                ItemType = dto.ItemType,
                ItemId = dto.ItemId,
                ItemCode = dto.ItemCode,
                ItemName = dto.ItemName,
                BrandLabel = dto.BrandLabel,
                CountValue = dto.CountValue,
                Quantity = dto.Quantity,
                WinderId = dto.WinderId,
                AssignedBy = userId,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Note = dto.Note
            };

            _winderContext.Tasks.Add(task);
            await _winderContext.SaveChangesAsync();
            createdTasks.Add(task.Id);
        }

        _logger.LogInformation($"Создано {createdTasks.Count} заданий пользователем {userId}");

        return Ok(new
        {
            message = $"Создано {createdTasks.Count} заданий",
            taskIds = createdTasks
        });
    }
}