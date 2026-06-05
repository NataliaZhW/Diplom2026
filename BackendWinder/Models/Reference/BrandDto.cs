namespace BackendWinder.Models.Reference
{
    // DTO для передачи данных о бренде клиенту
    public class BrandDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}