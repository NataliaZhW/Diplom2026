using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Mappers;
using BackendWinder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Services;

public class ReferenceService : IReferenceService
{
    private readonly OutsideContext _outsideContext;
    private readonly WinderContext _winderContext;
    private readonly ILogger<ReferenceService> _logger;

    public ReferenceService(
        OutsideContext outsideContext,
        WinderContext winderContext,
        ILogger<ReferenceService> logger)
    {
        _outsideContext = outsideContext;
        _winderContext = winderContext;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _outsideContext.Users.ToListAsync();
        return users.ToDtoList();
    }

    public async Task<List<ColorDto>> GetColorsAsync()
    {
        var colors = await _outsideContext.ColorThreads.ToListAsync();
        return colors.ToDtoList();
    }

    public async Task<List<BrandDto>> GetBrandsAsync()
    {
        var brands = await _winderContext.BrandManufs.ToListAsync();
        return brands.ToDtoList();
    }
}