using BackendWinder.DTOs;
using BackendWinder.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReferenceController : ControllerBase
{
    private readonly IReferenceService _referenceService;
    private readonly ILogger<ReferenceController> _logger;

    public ReferenceController(IReferenceService referenceService, ILogger<ReferenceController> logger)
    {
        _referenceService = referenceService;
        _logger = logger;
    }

    [HttpGet("users")]
    [Authorize(Roles = "master")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _referenceService.GetUsersAsync();
        return Ok(users);
    }

    [HttpGet("colors")]
    [AllowAnonymous]
    public async Task<IActionResult> GetColors()
    {
        var colors = await _referenceService.GetColorsAsync();
        return Ok(colors);
    }

    [HttpGet("brands")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _referenceService.GetBrandsAsync();
        return Ok(brands);
    }
}