using BackendWinder.DTOs;
using BackendWinder.Models.Outside;

namespace BackendWinder.Mappers;

/// <summary>
/// Методы расширения для маппинга KitComposition → KitCompositionDto
/// </summary>
public static class KitCompositionMapper
{
    public static KitCompositionDto ToDto(this KitComposition comp)
    {
        if (comp == null) return null!;

        return new KitCompositionDto
        {
            Id = comp.Id,
            Icon = comp.Icon?.IconValue ?? string.Empty,
            ColorId = comp.Color?.Id ?? 0,
            ColorCode = comp.Color?.Code ?? string.Empty,
            ColorName = comp.Color?.Name ?? string.Empty,
            Pnk = comp.Color?.Pnk,
            Dmc = comp.Color?.Dmc,
            Meterage = comp.Meterage,
            Count252 = comp.Count252,
            Count272 = comp.Count272,
            Count283 = comp.Count283,
            Count282 = comp.Count282,
            Count302 = comp.Count302,
            Count322 = comp.Count322,
            Count362 = comp.Count362,
            Count401 = comp.Count401,
            Note = comp.Note
        };
    }

    public static List<KitCompositionDto> ToDtoList(this IEnumerable<KitComposition> compositions)
    {
        return compositions.Select(c => c.ToDto()).ToList();
    }
}