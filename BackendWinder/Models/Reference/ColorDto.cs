namespace BackendWinder.Models.Reference
{
    // DTO для передачи данных о цвете клиенту
    public class ColorDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}