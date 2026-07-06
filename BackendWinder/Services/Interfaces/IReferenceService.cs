using BackendWinder.DTOs;

namespace BackendWinder.Services.Interfaces;

/// <summary>
/// Сервис для работы со справочниками
/// </summary>
public interface IReferenceService
{
    /// <summary>
    /// Получить список пользователей
    /// </summary>
    Task<List<UserDto>> GetUsersAsync();

    /// <summary>
    /// Получить список цветов
    /// </summary>
    Task<List<ColorDto>> GetColorsAsync();

    /// <summary>
    /// Получить список брендов
    /// </summary>
    Task<List<BrandDto>> GetBrandsAsync();
}