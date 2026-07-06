using BackendWinder.DTOs;
using BackendWinder.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ICatalogService catalogService, ILogger<CatalogController> logger)
    {
        _catalogService = catalogService;
        _logger = logger;
    }

    [HttpGet("icons")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIcons()
    {
        var icons = await _catalogService.GetIconsAsync();
        return Ok(icons);
    }

    [HttpGet("kits")]
    public async Task<IActionResult> GetKits([FromQuery] string? type, [FromQuery] string? search)
    {
        var kits = await _catalogService.GetKitsAsync(type, search);
        return Ok(kits);
    }

    [HttpGet("kits/{id}")]
    public async Task<IActionResult> GetKitById(int id)
    {
        var kit = await _catalogService.GetKitByIdAsync(id);
        if (kit == null)
            return NotFound(new { message = "Kit не найден" });

        return Ok(kit);
    }

    [HttpGet("threads")]
    [AllowAnonymous]
    public async Task<IActionResult> GetThreads([FromQuery] string? brand, [FromQuery] string? search)
    {
        var threads = await _catalogService.GetThreadsAsync(brand, search);
        return Ok(threads);
    }
}