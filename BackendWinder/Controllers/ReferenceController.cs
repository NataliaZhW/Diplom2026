using BackendWinder.Data;
using BackendWinder.Models.Outside;
using BackendWinder.Models.Winder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Controllers;

/// <summary>
/// Контроллер для просмотра справочных данных
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Требует авторизации для всех методов
public class ReferenceController : ControllerBase
{
    private readonly OutsideContext _outsideContext;
    private readonly WinderContext _winderContext;
    private readonly ILogger<ReferenceController> _logger;

    public ReferenceController(
        OutsideContext outsideContext,
        WinderContext winderContext,
        ILogger<ReferenceController> logger)
    {
        _outsideContext = outsideContext;
        _winderContext = winderContext;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    [HttpGet("users")]
    [Authorize(Roles = "master")] // Только для мастеров
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            _logger.LogInformation("Запрос списка пользователей");

            var users = await _outsideContext.Users
                .Select(u => new
                {
                    u.Id,
                    u.Login,
                    u.FullName,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка пользователей");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    /// <summary>
    /// Получить список всех цветов
    /// </summary>
    /// <returns>Список цветов</returns>
    [HttpGet("colors")]
    [AllowAnonymous] // Доступно всем (даже без авторизации)
    public async Task<IActionResult> GetColors()
    {
        try
        {
            _logger.LogInformation("Запрос списка цветов");

            var colors = await _outsideContext.ColorThreads
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Pnk,
                    c.Dmc
                })
                .ToListAsync();

            return Ok(colors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка цветов");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    /// <summary>
    /// Получить список всех брендов
    /// </summary>
    /// <returns>Список брендов</returns>
    [HttpGet("brands")]
    [AllowAnonymous] // Доступно всем (даже без авторизации)
    public async Task<IActionResult> GetBrands()
    {
        try
        {
            _logger.LogInformation("Запрос списка брендов");

            var brands = await _winderContext.BrandManufs
                .Select(b => new
                {
                    b.Id,
                    b.Code,
                    b.Name
                })
                .ToListAsync();

            return Ok(brands);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка брендов");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
}