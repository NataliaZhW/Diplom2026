namespace BackendWinder.Models.Config;

/// <summary>
/// Настройки бизнес-правил
/// </summary>
public class BusinessRulesConfig
{
    public int LabelsPerMeter { get; set; }
    public int[] AvailableCounts { get; set; } = Array.Empty<int>();
}