namespace BackendWinder.Models.Materials
{
    // DTO для передачи данных о запросе материалов клиенту
    public class MaterialRequestDto
    {
        public int Id { get; set; }
        public string TaskIds { get; set; } = string.Empty;  // JSON массив ID заданий
        public string RequestedByUserId { get; set; } = string.Empty;
        public string Materials { get; set; } = string.Empty;  // JSON с материалами
        public string Status { get; set; } = string.Empty;  // Pending, Received, Cancelled
        public DateTime RequestedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string? Notes { get; set; }
    }
}