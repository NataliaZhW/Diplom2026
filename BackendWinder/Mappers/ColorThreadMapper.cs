using BackendWinder.DTOs;
using BackendWinder.Models.Outside;

namespace BackendWinder.Mappers;

/// <summary>
/// Методы расширения для маппинга ColorThread → ColorDto
/// </summary>
public static class ColorThreadMapper
{
    public static ColorDto ToDto(this ColorThread color)
    {
        if (color == null) return null!;

        return new ColorDto
        {
            Id = color.Id,
            Code = color.Code,
            Name = color.Name,
            Pnk = color.Pnk,
            Dmc = color.Dmc
        };
    }

    public static List<ColorDto> ToDtoList(this IEnumerable<ColorThread> colors)
    {
        return colors.Select(c => c.ToDto()).ToList();
    }
}