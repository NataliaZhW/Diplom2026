using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Data;
using BackendWinder.Models.Reference;

namespace BackendWinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Все эндпоинты требуют авторизации
    public class ReferenceController : ControllerBase
    {
        private readonly ReferenceDbContext _referenceContext;

        public ReferenceController(ReferenceDbContext referenceContext)
        {
            _referenceContext = referenceContext;
        }

        // ================================================================
        // 1. ПОЛУЧИТЬ ВСЕ БРЕНДЫ
        // GET /api/reference/brands
        // ================================================================
        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _referenceContext.BrandManufs
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name
                })
                .ToListAsync();

            return Ok(brands);
        }

        // ================================================================
        // 2. ПОЛУЧИТЬ ВСЕ ЦВЕТА
        // GET /api/reference/colors
        // ================================================================
        [HttpGet("colors")]
        public async Task<IActionResult> GetColors()
        {
            var colors = await _referenceContext.ColorThreads
                .Select(c => new ColorDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(colors);
        }

        // ================================================================
        // 3. ПОЛУЧИТЬ ВСЕ КАУНТЫ
        // GET /api/reference/count-types
        // ================================================================
        [HttpGet("count-types")]
        public async Task<IActionResult> GetCountTypes()
        {
            var countTypes = await _referenceContext.CountTypes
                .Select(ct => new CountTypeDto
                {
                    Id = ct.Id,
                    Code = ct.Code,
                    DisplayName = ct.DisplayName,
                    ThreadCount = ct.ThreadCount,
                    LabelColor = ct.LabelColor
                })
                .ToListAsync();

            return Ok(countTypes);
        }

        // ================================================================
        // 4. ПОЛУЧИТЬ ВСЕ КОМПЛЕКТЫ
        // GET /api/reference/kits-schemes
        // ================================================================
        [HttpGet("kits-schemes")]
        public async Task<IActionResult> GetKitsSchemes()
        {
            var kitsSchemes = await _referenceContext.KitsSchemes
                .Select(ks => new KitSchemeDto
                {
                    Id = ks.Id,
                    Code = ks.Code,
                    Name = ks.Name
                })
                .ToListAsync();

            return Ok(kitsSchemes);
        }

        // ================================================================
        // 5. ПОЛУЧИТЬ ДЕТАЛИ КОМПЛЕКТА ПО КОДУ
        // GET /api/reference/kits-schemes/{code}
        // ================================================================
        [HttpGet("kits-schemes/{code}")]
        public async Task<IActionResult> GetKitSchemeDetails(string code)
        {
            // Находим комплект по коду
            var kitScheme = await _referenceContext.KitsSchemes
                .FirstOrDefaultAsync(ks => ks.Code == code);

            if (kitScheme == null)
            {
                return NotFound(new { message = $"Комплект с кодом '{code}' не найден" });
            }

            // Получаем состав комплекта (какие нити входят)
            var compositions = await _referenceContext.KitSchemeCompositions
                .Where(c => c.KitSchemeId == kitScheme.Id)
                .Select(c => new
                {
                    c.Id,
                    c.ThreadType,
                    RegularThread = c.RegularThread != null ? new { c.RegularThread.Code, c.RegularThread.Name } : null,
                    PerleThread = c.PerleThread != null ? new { c.PerleThread.Code, c.PerleThread.Name } : null,
                    MetallicThread = c.MetallicThread != null ? new { c.MetallicThread.Code, c.MetallicThread.Name } : null,
                    Brand = c.BrandManuf != null ? new { c.BrandManuf.Code, c.BrandManuf.Name } : null
                })
                .ToListAsync();

            // Получаем варианты схем (с каунтами и количеством пасм)
            var schemeVariants = await _referenceContext.SchemeVariants
                .Where(sv => sv.KitSchemeId == kitScheme.Id && sv.IsAvailable)
                .Include(sv => sv.CountType)
                .Select(sv => new
                {
                    sv.Id,
                    CountType = new { sv.CountType!.Code, sv.CountType.DisplayName },
                    Details = _referenceContext.SchemeVariantDetails
                        .Where(d => d.SchemeVariantId == sv.Id)
                        .Select(d => new
                        {
                            CompositionId = d.CompositionId,
                            Quantity = d.Quantity
                        })
                        .ToList()
                })
                .ToListAsync();

            // Получаем варианты наборов (с метражом)
            var kitVariants = await _referenceContext.KitVariants
                .Where(kv => kv.KitSchemeId == kitScheme.Id && kv.IsAvailable)
                .Select(kv => new
                {
                    kv.Id,
                    Details = _referenceContext.KitVariantDetails
                        .Where(d => d.KitVariantId == kv.Id)
                        .Select(d => new
                        {
                            CompositionId = d.CompositionId,
                            Quantity = d.Quantity
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(new
            {
                kitScheme.Id,
                kitScheme.Code,
                kitScheme.Name,
                Compositions = compositions,
                SchemeVariants = schemeVariants,
                KitVariants = kitVariants
            });
        }
    }
}