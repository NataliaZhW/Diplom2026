using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Reference
{
    public class KitVariantDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int KitVariantId { get; set; }
        public int CompositionId { get; set; }
        public int Quantity { get; set; }
        
        [ForeignKey("KitVariantId")]
        public KitVariant? KitVariant { get; set; }
        
        [ForeignKey("CompositionId")]
        public KitSchemeComposition? Composition { get; set; }
    }
}