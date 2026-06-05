namespace BackendWinder.Models.Tasks
{
    // DTO для передачи данных о задании клиенту
    public class TaskDto
    {
        public int Id { get; set; }
        public int? PlannedTaskId { get; set; }
        public string AssignedToUserId { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;  // Scheme, Kit, Thread
        public string? KitSchemeCode { get; set; }
        public string? KitSchemeName { get; set; }
        public string? BrandCode { get; set; }
        public string? ColorCode { get; set; }
        public int? CountCode { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? MaterialsRequestedAt { get; set; }
        public DateTime? MaterialsReceivedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReportedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}