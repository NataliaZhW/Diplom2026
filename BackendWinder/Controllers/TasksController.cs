using BackendWinder.DTOs;
using BackendWinder.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    private string GetUserRole() =>
        User.FindFirst(ClaimTypes.Role)?.Value ?? "winder";

    // ============================================================
    // 1. Получить все задания
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var tasks = await _taskService.GetTasksAsync(userId, userRole);
        return Ok(tasks);
    }

    // ============================================================
    // 2. Получить задание по ID
    // ============================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var task = await _taskService.GetTaskByIdAsync(id, userId, userRole);
        if (task == null)
            return NotFound(new { message = "Задание не найдено" });

        return Ok(task);
    }

    // ============================================================
    // 3. Создать задание
    // ============================================================
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        if (userRole != "master")
            return Forbid();

        var task = await _taskService.CreateTaskAsync(dto, userId);
        return Ok(new { id = task.Id, message = "Задание создано" });
    }

    // ============================================================
    // 4. Обновить статус задания
    // ============================================================
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskStatusUpdateDto dto)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var success = await _taskService.UpdateStatusAsync(id, dto.Status, userId, userRole);
        if (!success)
            return BadRequest(new { message = "Не удалось обновить статус" });

        return Ok(new { message = "Статус обновлён" });
    }

    // ============================================================
    // 5. Удалить задание (только new)
    // ============================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userRole = GetUserRole();

        var success = await _taskService.DeleteTaskAsync(id, userRole);
        if (!success)
            return BadRequest(new { message = "Не удалось удалить задание" });

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

        var userId = GetUserId();
        var userRole = GetUserRole();

        var taskIds = await _taskService.CreateBatchTasksAsync(items, userId, userRole);
        if (taskIds.Count == 0 && userRole != "master")
            return Forbid();

        return Ok(new
        {
            message = $"Создано {taskIds.Count} заданий",
            taskIds = taskIds
        });
    }

    // ============================================================
    // 7. Принять задание (только для мастера)
    // ============================================================
    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptTask(int id)
    {
        try
        {
            var userRole = GetUserRole();
            var acceptedAt = await _taskService.AcceptTaskAsync(id, userRole);
            return Ok(new { message = "Задание принято", acceptedAt });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Задание не найдено" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при принятии задания {id}");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // ============================================================
    // 8. Отменить принятие (только для мастера)
    // ============================================================
    [HttpPost("{id}/cancel-accept")]
    public async Task<IActionResult> CancelAccept(int id)
    {
        var userRole = GetUserRole();
        var success = await _taskService.CancelAcceptAsync(id, userRole);

        if (!success)
            return BadRequest(new { message = "Нет принятия для отмены" });

        return Ok(new { message = "Принятие отменено" });
    }

    // ============================================================
    // 9. Массовый перевод заданий в статус "materials_requested"
    // ============================================================
    [HttpPost("batch/calculate-materials")]
    public async Task<IActionResult> CalculateMaterials([FromBody] List<int> taskIds)
    {
        try
        {
            if (taskIds == null || taskIds.Count == 0)
                return BadRequest(new { message = "Выберите задания" });

            var userId = GetUserId();
            var userRole = GetUserRole();

            var count = await _taskService.CalculateMaterialsAsync(taskIds, userId, userRole);

            return Ok(new { message = $"Переведено {count} заданий в статус 'Материалы запрошены'" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при массовом расчёте материалов");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // ============================================================
    // 10. Массовый перевод заданий в статус "reported"
    // ============================================================
    [HttpPost("batch/submit-report")]
    public async Task<IActionResult> SubmitReport([FromBody] List<int> taskIds)
    {
        try
        {
            if (taskIds == null || taskIds.Count == 0)
                return BadRequest(new { message = "Выберите задания" });

            var userId = GetUserId();
            var userRole = GetUserRole();

            var count = await _taskService.SubmitReportAsync(taskIds, userId, userRole);

            return Ok(new { message = $"Переведено {count} заданий в статус 'Внесено в отчетность'" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при массовом отчёте");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
}