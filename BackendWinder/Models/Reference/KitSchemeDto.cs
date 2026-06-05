namespace BackendWinder.Models.Reference
{
    // DTO для передачи данных о комплекте клиенту
    public class KitSchemeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}