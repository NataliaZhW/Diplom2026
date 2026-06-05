using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Reference
{
    public class SchemeVariantDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int SchemeVariantId { get; set; }
        public int CompositionId { get; set; }
        public int Quantity { get; set; }
        
        [ForeignKey("SchemeVariantId")]
        public SchemeVariant? SchemeVariant { get; set; }
        
        [ForeignKey("CompositionId")]
        public KitSchemeComposition? Composition { get; set; }
    }
}