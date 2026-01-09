using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public enum GravidadeAlergia
    {
        Leve,
        Moderada,
        Grave
    }
    public class Alergia
    {
        [Key]
        public int AlergiaId { get; set; }

        [Required(ErrorMessage = "O nome da alergia é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome da Alergia")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A gravidade é obrigatória.")]
        [Display(Name = "Gravidade")]
        public GravidadeAlergia Gravidade { get; set; }

        [Required(ErrorMessage = "Os sintomas são obrigatórios.")]
        [StringLength(300, ErrorMessage = "Os sintomas devem ter no máximo 300 caracteres.")]
        [Display(Name = "Sintomas")]
        public string Sintomas { get; set; }

        [Display(Name = "Alimento Associado")]
        
        public ICollection<AlergiaAlimento>? AlimentosAssociados { get; set; }
    }
}
