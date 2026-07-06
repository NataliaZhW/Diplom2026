using BackendWinder.DTOs;
using BackendWinder.Models.Winder;

namespace BackendWinder.Mappers;

/// <summary>
/// Методы расширения для маппинга BrandManuf → BrandDto
/// </summary>
public static class BrandManufMapper
{
    public static BrandDto ToDto(this BrandManuf brand)
    {
        if (brand == null) return null!;

        return new BrandDto
        {
            Id = brand.Id,
            Code = brand.Code,
            Name = brand.Name
        };
    }

    public static List<BrandDto> ToDtoList(this IEnumerable<BrandManuf> brands)
    {
        return brands.Select(b => b.ToDto()).ToList();
    }
}