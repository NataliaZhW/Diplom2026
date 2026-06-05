using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Reference
{
    public class CountType
    {
        [Key]
        public int Id { get; set; }
        
        public int Code { get; set; }
        
        [MaxLength(50)]
        public string DisplayName { get; set; } = string.Empty;
        
        public int ThreadCount { get; set; }
        
        [MaxLength(50)]
        public string LabelColor { get; set; } = string.Empty;
    }
}