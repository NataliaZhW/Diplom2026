using BackendWinder.DTOs;
using BackendWinder.Models.Winder;

namespace BackendWinder.Services.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с заданиями
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Получить все задания для пользователя
    /// </summary>
    Task<List<TaskDto>> GetTasksAsync(int userId, string userRole);

    /// <summary>
    /// Получить задание по ID
    /// </summary>
    Task<TaskDto?> GetTaskByIdAsync(int id, int userId, string userRole);

    /// <summary>
    /// Создать задание
    /// </summary>
    Task<WinderTask> CreateTaskAsync(TaskCreateDto dto, int assignedBy);

    /// <summary>
    /// Массовое создание заданий
    /// </summary>
    Task<List<int>> CreateBatchTasksAsync(List<TaskCreateDto> items, int assignedBy, string userRole);

    /// <summary>
    /// Обновить статус задания
    /// </summary>
    Task<bool> UpdateStatusAsync(int id, string newStatus, int userId, string userRole);

    /// <summary>
    /// Удалить задание
    /// </summary>
    Task<bool> DeleteTaskAsync(int id, string userRole);

    /// <summary>
    /// Принять задание (мастер)
    /// </summary>
    Task<DateTime> AcceptTaskAsync(int id, string userRole);

    /// <summary>
    /// Отменить принятие (мастер)
    /// </summary>
    Task<bool> CancelAcceptAsync(int id, string userRole);

    /// <summary>
    /// Массовый расчёт материалов (мотальщик)
    /// </summary>
    Task<int> CalculateMaterialsAsync(List<int> taskIds, int userId, string userRole);

    /// <summary>
    /// Массовый отчёт (мотальщик)
    /// </summary>
    Task<int> SubmitReportAsync(List<int> taskIds, int userId, string userRole);
}