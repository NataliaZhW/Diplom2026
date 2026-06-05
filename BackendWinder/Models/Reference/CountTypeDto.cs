namespace BackendWinder.Models.Reference
{
    // DTO для передачи данных о каунте клиенту
    public class CountTypeDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int ThreadCount { get; set; }
        public string LabelColor { get; set; } = string.Empty;
    }
}