namespace BackendWinder.DTOs;

public class ColorDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Pnk { get; set; }
    public string? Dmc { get; set; }
}