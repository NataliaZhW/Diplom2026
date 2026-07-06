using BackendWinder.DTOs;
using BackendWinder.Models.Outside;

namespace BackendWinder.Mappers;

/// <summary>
/// Методы расширения для маппинга Kit → KitDto
/// </summary>
public static class KitMapper
{
    // ДЛЯ ОДНОГО ОБЪЕКТА (с Include)
    public static KitDto ToDto(this Kit kit)
{
    if (kit == null) return null!;

        return new KitDto
        {
            Id = kit.Id,
            InternalCode = kit.InternalCode,
            Name = kit.Name,
            Note = kit.Note,
            // ✅ Определяем тип: если есть метраж → "kit", иначе "scheme"
            Type = kit.Compositions.Any(c => c.Meterage != null && c.Meterage > 0) ? "kit" : "scheme",
            Compositions = kit.Compositions.Select(c => c.ToDto()).ToList()
        };
    }

    // ДЛЯ СПИСКА (с Include)
    public static List<KitDto> ToDtoList(this IEnumerable<Kit> kits)
    {
        return kits.Select(k => k.ToDto()).ToList();
    }
}