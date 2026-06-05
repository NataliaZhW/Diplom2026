using BackendWinder.Models.Tasks;

namespace BackendWinder.Models.Admin
{
    // Ответ со списком всех заданий (для админа)
    public class AllTasksResponse
    {
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
        public int TotalCount { get; set; }
        public int NewCount { get; set; }
        public int InProgressCount { get; set; }
        public int SubmittedCount { get; set; }
        public int ArchivedCount { get; set; }
    }
}