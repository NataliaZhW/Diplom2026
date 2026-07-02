namespace BackendWinder.DTOs;

/// <summary>
/// DTO для отображения Kit (набор/схема)
/// </summary>
public class KitDto
{
    public int Id { get; set; }
    public string InternalCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string Type { get; set; } = string.Empty; // "kit" или "scheme"
    public List<KitCompositionDto> Compositions { get; set; } = new();
}

/// <summary>
/// DTO для состава Kit
/// </summary>
public class KitCompositionDto
{
    public int Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public int ColorId { get; set; }
    public string ColorCode { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string? Pnk { get; set; }
    public string? Dmc { get; set; }
    public decimal? Meterage { get; set; }
    public int Count252 { get; set; }
    public int Count272 { get; set; }
    public int Count283 { get; set; }
    public int Count282 { get; set; }
    public int Count302 { get; set; }
    public int Count322 { get; set; }
    public int Count362 { get; set; }
    public int Count401 { get; set; }
    public string? Note { get; set; }
    
    // Для расчета бирочек
    public int? LabelsCount => Meterage.HasValue ? (int)Math.Ceiling(Meterage.Value / 10) : null;
}

/// <summary>
/// DTO для отображения нити
/// </summary>
public class ThreadDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Pnk { get; set; }
    public string? Dmc { get; set; }
}