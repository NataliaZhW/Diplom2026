using BackendWinder.Data;
using BackendWinder.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogController : ControllerBase
{
    private readonly OutsideContext _context;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(OutsideContext context, ILogger<CatalogController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ============================================================
    // 1. Получить все значки
    // ============================================================
    [HttpGet("icons")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIcons()
    {
        var icons = await _context.Icons
            .Select(i => i.IconValue)
            .ToListAsync();
        return Ok(icons);
    }

    // ============================================================
    // 2. Получить все Kit (наборы/схемы) с фильтрацией
    // ============================================================
    [HttpGet("kits")]
    public async Task<IActionResult> GetKits([FromQuery] string? type, [FromQuery] string? search)
    {
        var query = _context.Kits
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Icon)
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Color)
            .AsQueryable();

        // Фильтр по типу
        if (!string.IsNullOrEmpty(type))
        {
            if (type == "kit")
            {
                // Набор: есть метраж у любого состава
                query = query.Where(k => k.Compositions.Any(c => c.Meterage != null && c.Meterage > 0));
            }
            else if (type == "scheme")
            {
                // Схема: есть любой каунт > 0 у любого состава
                query = query.Where(k => k.Compositions.Any(c =>
                    c.Count252 > 0 || c.Count272 > 0 || c.Count283 > 0 ||
                    c.Count282 > 0 || c.Count302 > 0 || c.Count322 > 0 ||
                    c.Count362 > 0 || c.Count401 > 0
                ));
            }
        }

        // Поиск по номеру или названию
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(k =>
                k.InternalCode.Contains(search) ||
                k.Name.Contains(search)
            );
        }

        var kits = await query
            .OrderBy(k => k.InternalCode)
            .Select(k => new KitDto
            {
                Id = k.Id,
                InternalCode = k.InternalCode,
                Name = k.Name,
                Note = k.Note,
                // Определяем тип для отображения
                Type = k.Compositions.Any(c => c.Meterage != null && c.Meterage > 0) ? "kit" : "scheme",
                Compositions = k.Compositions.Select(c => new KitCompositionDto
                {
                    Id = c.Id,
                    Icon = c.Icon.IconValue,
                    ColorId = c.Color.Id,
                    ColorCode = c.Color.Code,
                    ColorName = c.Color.Name,
                    Pnk = c.Color.Pnk,
                    Dmc = c.Color.Dmc,
                    Meterage = c.Meterage,
                    Count252 = c.Count252,
                    Count272 = c.Count272,
                    Count283 = c.Count283,
                    Count282 = c.Count282,
                    Count302 = c.Count302,
                    Count322 = c.Count322,
                    Count362 = c.Count362,
                    Count401 = c.Count401,
                    Note = c.Note
                }).ToList()
            })
            .ToListAsync();

        return Ok(kits);
    }

    // ============================================================
    // 3. Получить Kit по ID
    // ============================================================
    [HttpGet("kits/{id}")]
    public async Task<IActionResult> GetKitById(int id)
    {
        var kit = await _context.Kits
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Icon)
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Color)
            .Where(k => k.Id == id)
            .Select(k => new KitDto
            {
                Id = k.Id,
                InternalCode = k.InternalCode,
                Name = k.Name,
                Note = k.Note,
                Type = k.Compositions.Any(c => c.Meterage != null) ? "kit" : "scheme",
                Compositions = k.Compositions.Select(c => new KitCompositionDto
                {
                    Id = c.Id,
                    Icon = c.Icon.IconValue,
                    ColorId = c.Color.Id,
                    ColorCode = c.Color.Code,
                    ColorName = c.Color.Name,
                    Pnk = c.Color.Pnk,
                    Dmc = c.Color.Dmc,
                    Meterage = c.Meterage,
                    Count252 = c.Count252,
                    Count272 = c.Count272,
                    Count283 = c.Count283,
                    Count282 = c.Count282,
                    Count302 = c.Count302,
                    Count322 = c.Count322,
                    Count362 = c.Count362,
                    Count401 = c.Count401,
                    Note = c.Note
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (kit == null)
            return NotFound(new { message = "Kit не найден" });

        return Ok(kit);
    }

    // ============================================================
    // 4. Получить все нити (ColorThreads) с фильтром
    // ============================================================
    [HttpGet("threads")]
    [AllowAnonymous]
    public async Task<IActionResult> GetThreads([FromQuery] string? brand, [FromQuery] string? search)
    {
        var query = _context.ColorThreads.AsQueryable();

        // Фильтр по бренду
        if (!string.IsNullOrEmpty(brand))
        {
            if (brand.ToLower() == "pnk")
                query = query.Where(c => c.Pnk != null && c.Pnk != "-");
            else if (brand.ToLower() == "dmc")
                query = query.Where(c => c.Dmc != null && c.Dmc != "-");
        }

        // Поиск по коду или названию
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c =>
                c.Code.Contains(search) ||
                c.Name.Contains(search)
            );
        }

        var threads = await query
            .OrderBy(c => c.Code)
            .Select(c => new ThreadDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Pnk = c.Pnk,
                Dmc = c.Dmc
            })
            .ToListAsync();

        return Ok(threads);
    }
}