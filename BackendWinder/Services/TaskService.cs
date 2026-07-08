using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Models.Winder;
using BackendWinder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Services;

/// <summary>
/// Сервис для работы с заданиями
/// </summary>
public class TaskService : ITaskService
{
    private readonly WinderContext _winderContext;
    private readonly OutsideContext _outsideContext;
    private readonly ILogger<TaskService> _logger;

    // Статусы и их лейблы
    private static readonly Dictionary<string, string> StatusLabels = new()
    {
        { "new", "Новое" },
        { "materials_requested", "Материалы запрошены" },
        { "materials_received", "Материалы получены" },
        { "submitted", "Сдано" },
        { "reported", "Внесено в отчетность" },
        { "archived", "В архиве" }
    };

    private static readonly Dictionary<string, string> TypeLabels = new()
    {
        { "kit", "Набор" },
        { "scheme", "Схема" },
        { "thread", "Нить" }
    };

    public TaskService(
        WinderContext winderContext,
        OutsideContext outsideContext,
        ILogger<TaskService> logger)
    {
        _winderContext = winderContext;
        _outsideContext = outsideContext;
        _logger = logger;
    }

    public async Task<List<TaskDto>> GetTasksAsync(int userId, string userRole)
    {
        var query = _winderContext.Tasks.AsQueryable();

        if (userRole != "master")
        {
            query = query.Where(t => t.WinderId == userId);
        }

        var tasksFromDb = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        // Собираем все ID пользователей (и мотальщиков, и тех, кто назначил)
        var allUserIds = new HashSet<int>();
        foreach (var task in tasksFromDb)
        {
            allUserIds.Add(task.WinderId);
            if (task.AssignedBy.HasValue)
                allUserIds.Add(task.AssignedBy.Value);
        }

        var userNames = new Dictionary<int, string>();
        if (allUserIds.Any())
        {
            var users = await _outsideContext.Users
                .Where(u => allUserIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            foreach (var user in users)
            {
                userNames[user.Id] = user.FullName;
            }
        }

        return tasksFromDb.Select(t => new TaskDto
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
            AssignedByName = userNames.GetValueOrDefault(t.AssignedBy ?? 0, "Неизвестно"), 
            CreatedAt = t.CreatedAt,
            AssignedAt = t.AssignedAt,
            MaterialsRequestedAt = t.MaterialsRequestedAt,
            MaterialsIssuedAt = t.MaterialsIssuedAt,
            SubmittedAt = t.SubmittedAt,
            AcceptedAt = t.AcceptedAt,
            ReportedAt = t.ReportedAt,
            ArchivedAt = t.ArchivedAt,
            Note = t.Note
        }).ToList();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id, int userId, string userRole)
    {
        var task = await _winderContext.Tasks
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();

        if (task == null)
            return null;

        if (userRole != "master" && task.WinderId != userId)
            return null;

        var winderName = "Неизвестно";
        var user = await _outsideContext.Users
            .Where(u => u.Id == task.WinderId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync();

        if (user != null)
            winderName = user;

        return new TaskDto
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
            MaterialsRequestedAt = task.MaterialsRequestedAt,
            MaterialsIssuedAt = task.MaterialsIssuedAt,
            SubmittedAt = task.SubmittedAt,
            AcceptedAt = task.AcceptedAt,
            ReportedAt = task.ReportedAt,
            ArchivedAt = task.ArchivedAt,
            Note = task.Note
        };
    }

    public async Task<WinderTask> CreateTaskAsync(TaskCreateDto dto, int assignedBy)
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
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Note = dto.Note
        };

        _winderContext.Tasks.Add(task);
        await _winderContext.SaveChangesAsync();

        _logger.LogInformation($"Создано задание {task.Id} для пользователя {task.WinderId}");
        return task;
    }

    public async Task<List<int>> CreateBatchTasksAsync(List<TaskCreateDto> items, int assignedBy, string userRole)
    {
        if (userRole != "master")
        {
            var hasOtherWinder = items.Any(item => item.WinderId != assignedBy);
            if (hasOtherWinder)
            {
                _logger.LogWarning($"Мотальщик {assignedBy} попытался создать задание для другого пользователя");
                return new List<int>();
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
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Note = dto.Note
            };

            _winderContext.Tasks.Add(task);
            await _winderContext.SaveChangesAsync();
            createdTasks.Add(task.Id);
        }

        _logger.LogInformation($"Создано {createdTasks.Count} заданий пользователем {assignedBy}");
        return createdTasks;
    }

    public async Task<bool> UpdateStatusAsync(int id, string newStatus, int userId, string userRole)
    {
        var task = await _winderContext.Tasks.FindAsync(id);
        if (task == null)
            return false;

        if (userRole != "master" && task.WinderId != userId)
            return false;

        task.Status = newStatus;

        switch (newStatus)
        {
            case "materials_requested":
                task.MaterialsRequestedAt = DateTime.UtcNow;
                break;
            case "materials_received":
                task.MaterialsIssuedAt = DateTime.UtcNow;
                break;
            case "submitted":
                task.SubmittedAt = DateTime.UtcNow;
                break;
            case "reported":
                task.ReportedAt = DateTime.UtcNow;
                break;
            case "archived":
                task.ArchivedAt = DateTime.UtcNow;
                break;
        }

        await _winderContext.SaveChangesAsync();
        _logger.LogInformation($"Обновлён статус задания {id} на {newStatus}");
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int id, string userRole)
    {
        if (userRole != "master")
            return false;

        var task = await _winderContext.Tasks.FindAsync(id);
        if (task == null)
            return false;

        if (task.Status != "new")
            return false;

        _winderContext.Tasks.Remove(task);
        await _winderContext.SaveChangesAsync();
        _logger.LogInformation($"Удалено задание {id}");
        return true;
    }

    public async Task<DateTime> AcceptTaskAsync(int id, string userRole)
    {
        if (userRole != "master")
            throw new UnauthorizedAccessException("Только мастер может принимать задания");

        var task = await _winderContext.Tasks.FindAsync(id);
        if (task == null)
            throw new KeyNotFoundException("Задание не найдено");

        if (task.Status != "submitted" && task.Status != "reported" && task.Status != "archived")
            throw new InvalidOperationException("Можно принять только задания в статусе 'Сдано', 'Внесено в отчетность' или 'В архиве'");

        // МАСТЕР ЗАПИСЫВАЕТ В accepted_at
        if (task.AcceptedAt != null)
            throw new InvalidOperationException("Задание уже принято");

        task.AcceptedAt = DateTime.UtcNow;

        await _winderContext.SaveChangesAsync();
        _logger.LogInformation($"Задание {id} принято мастером");

        return task.AcceptedAt.Value;
    }

    public async Task<bool> CancelAcceptAsync(int id, string userRole)
    {
        if (userRole != "master")
            return false;

        var task = await _winderContext.Tasks.FindAsync(id);
        if (task == null)
            return false;

        // МАСТЕР ОЧИЩАЕТ accepted_at
        if (task.AcceptedAt == null)
            return false;

        task.AcceptedAt = null;

        await _winderContext.SaveChangesAsync();
        _logger.LogInformation($"Принятие задания {id} отменено");
        return true;
    }

    public async Task<int> CalculateMaterialsAsync(List<int> taskIds, int userId, string userRole)
    {
        if (userRole == "master")
            throw new UnauthorizedAccessException("Только мотальщик может рассчитывать материалы");

        var tasks = await _winderContext.Tasks
            .Where(t => taskIds.Contains(t.Id))
            .ToListAsync();

        if (tasks.Count == 0)
            throw new KeyNotFoundException("Задания не найдены");

        var invalidTasks = tasks.Where(t => t.Status != "new").ToList();
        if (invalidTasks.Any())
        {
            throw new InvalidOperationException(
                $"Задания {string.Join(", ", invalidTasks.Select(t => t.Id))} не в статусе 'Новое'");
        }

        foreach (var task in tasks)
        {
            task.Status = "materials_requested";
            task.MaterialsRequestedAt = DateTime.UtcNow;
        }

        await _winderContext.SaveChangesAsync();
        _logger.LogInformation($"Переведено {tasks.Count} заданий в 'Материалы запрошены' пользователем {userId}");

        return tasks.Count;
    }

    public async Task<int> SubmitReportAsync(List<int> taskIds, int userId, string userRole)
    {
        if (userRole == "master")
            throw new UnauthorizedAccessException("Только мотальщик может отправлять в отчёт");

        var tasks = await _winderContext.Tasks
            .Where(t => taskIds.Contains(t.Id))
            .ToListAsync();

        if (tasks.Count == 0)
            throw new KeyNotFoundException("Задания не найдены");

        var invalidTasks = tasks.Where(t => t.Status != "submitted").ToList();
        if (invalidTasks.Any())
        {
            throw new InvalidOperationException(
                $"Задания {string.Join(", ", invalidTasks.Select(t => t.Id))} не в статусе 'Сдано'");
        }

        foreach (var task in tasks)
        {
            task.Status = "reported";
            task.ReportedAt = DateTime.UtcNow;
        }

        await _winderContext.SaveChangesAsync();
        _logger.LogInformation($"Переведено {tasks.Count} заданий в 'Внесено в отчетность' пользователем {userId}");

        return tasks.Count;
    }
}