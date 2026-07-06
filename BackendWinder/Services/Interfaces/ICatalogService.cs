using BackendWinder.DTOs;

namespace BackendWinder.Services.Interfaces;

/// <summary>
/// Сервис для работы с каталогом (наборы, схемы, нити)
/// </summary>
public interface ICatalogService
{
    /// <summary>
    /// Получить все значки
    /// </summary>
    Task<List<string>> GetIconsAsync();

    /// <summary>
    /// Получить все Kit (наборы/схемы) с фильтрацией
    /// </summary>
    Task<List<KitDto>> GetKitsAsync(string? type, string? search);

    /// <summary>
    /// Получить Kit по ID
    /// </summary>
    Task<KitDto?> GetKitByIdAsync(int id);

    /// <summary>
    /// Получить все нити с фильтрацией по бренду
    /// </summary>
    Task<List<ThreadDto>> GetThreadsAsync(string? brand, string? search);
}