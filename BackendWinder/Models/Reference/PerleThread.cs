using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Reference
{
    public class PerleThread
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}