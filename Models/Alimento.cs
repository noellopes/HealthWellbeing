using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Alimento
    {
        [Key]
        public int AlimentoId { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; }
        public int CategoriaAlimentoId { get; set; }               
        public int Calories { get; set; }
        public CategoriaAlimento Categoria { get; set; } = null!;   

        [Range(0, 1000)] public decimal KcalPor100g { get; set; }
        [Range(0, 100)] public decimal ProteinaGPor100g { get; set; }
        [Range(0, 100)] public decimal HidratosGPor100g { get; set; }
        [Range(0, 100)] public decimal GorduraGPor100g { get; set; }

        public ICollection<Alergia>? AlergiaRelacionadas { get; set; }
        public ICollection<AlimentoSubstituto>? Substitutos { get; set; }
        public ICollection<AlimentoSubstituto>? SubstituidoPor { get; set; }
    }
}
