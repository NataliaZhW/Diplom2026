using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Reference
{
    public class KitSchemeComposition
    {
        [Key]
        public int Id { get; set; }
        
        public int KitSchemeId { get; set; }
        
        [MaxLength(20)]
        public string ThreadType { get; set; } = string.Empty; // "Regular", "Perle", "Metallic"
        
        public int? RegularThreadId { get; set; }
        public int? PerleThreadId { get; set; }
        public int? MetallicThreadId { get; set; }
        public int? BrandManufId { get; set; }
        
        [ForeignKey("KitSchemeId")]
        public KitScheme? KitScheme { get; set; }
        
        [ForeignKey("RegularThreadId")]
        public ColorThread? RegularThread { get; set; }
        
        [ForeignKey("PerleThreadId")]
        public PerleThread? PerleThread { get; set; }
        
        [ForeignKey("MetallicThreadId")]
        public MetallicThread? MetallicThread { get; set; }
        
        [ForeignKey("BrandManufId")]
        public BrandManuf? BrandManuf { get; set; }
    }
}