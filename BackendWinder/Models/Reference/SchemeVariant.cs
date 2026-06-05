using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Reference
{
    public class SchemeVariant
    {
        [Key]
        public int Id { get; set; }
        
        public int KitSchemeId { get; set; }
        public int CountTypeId { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        [ForeignKey("KitSchemeId")]
        public KitScheme? KitScheme { get; set; }
        
        [ForeignKey("CountTypeId")]
        public CountType? CountType { get; set; }
    }
}