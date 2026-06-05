using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Reference
{
    public class KitScheme
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}