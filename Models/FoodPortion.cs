using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodPortion
    {
        [Key]
        public int FoodPortionId { get; set; }

        // Relação com o alimento
        [Required]
        [Display(Name = "Food")]
        public int FoodId { get; set; }
        public Food? Food { get; set; }

        // Nome ou descrição da porção (ex: "1 cup cooked (150 g)")
        [Required]
        [StringLength(80)]
        [Display(Name = "Portion")]
        public string Portion { get; set; } = string.Empty;

        // Quantidade equivalente em gramas ou ml
        [Range(1, 9999)]
        [Column(TypeName = "decimal(9,2)")]
        [Display(Name = "Amount (g/ml)")]
        public decimal AmountGramsMl { get; set; }
    }
}
