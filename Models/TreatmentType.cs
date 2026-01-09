using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TreatmentType
    {

        //Propriedades
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório!")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        [Display(Name = "Nome")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória!")]
        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
        [Display(Name = "Descrição")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "A duração estimada é obrigatória!")]
        [Range(1, 480, ErrorMessage = "A duração estimada deve estar entre 1 e 480 minutos.")]
        [Display(Name = "Duração Estimada")]
        public required int EstimatedDuration { get; set; }

    }

}
