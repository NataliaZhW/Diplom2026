using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Mappers;
using BackendWinder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Models.Config;

namespace BackendWinder.Services;

public class CatalogService : ICatalogService
{
    private readonly OutsideContext _context;
    private readonly ILogger<CatalogService> _logger;

    private readonly BusinessRulesConfig _businessRules;

    public CatalogService(
        OutsideContext context,
        ILogger<CatalogService> logger,
        BusinessRulesConfig businessRules)
    {
        _context = context;
        _logger = logger;
        _businessRules = businessRules;
    }

    public async Task<List<string>> GetIconsAsync()
    {
        return await _context.Icons
            .Select(i => i.IconValue)
            .ToListAsync();
    }

    public async Task<List<KitDto>> GetKitsAsync(string? type, string? search)
    {
        var query = _context.Kits
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Icon)
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Color)
            .AsQueryable();

        // ============================================================
        // ФИЛЬТР ПО ТИПУ:
        // - "kit"   → есть метраж (Meterage != null && Meterage > 0)
        // - "scheme" → есть каунты (любой Count_* > 0)
        // ============================================================
        if (!string.IsNullOrEmpty(type))
        {
            if (type == "kit")
            {
                query = query.Where(k => k.Compositions.Any(c => c.Meterage != null && c.Meterage > 0));
            }
            else if (type == "scheme")
            {
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
            .ToListAsync();

        return kits.ToDtoList();
    }

    public async Task<KitDto?> GetKitByIdAsync(int id)
    {
        // Include + метод расширения 
        var kit = await _context.Kits
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Icon)
            .Include(k => k.Compositions)
                .ThenInclude(c => c.Color)
            .FirstOrDefaultAsync(k => k.Id == id);

        return kit?.ToDto();
    }

    public async Task<List<ThreadDto>> GetThreadsAsync(string? brand, string? search)
    {
        var query = _context.ColorThreads.AsQueryable();

        if (!string.IsNullOrEmpty(brand))
        {
            if (brand.ToLower() == "pnk")
                query = query.Where(c => c.Pnk != null && c.Pnk != "-");
            else if (brand.ToLower() == "dmc")
                query = query.Where(c => c.Dmc != null && c.Dmc != "-");
        }

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

        return threads;
    }
}