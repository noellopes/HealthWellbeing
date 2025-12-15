using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ActivityType
    {
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome do Tipo")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(200, ErrorMessage = "A descrição não pode ter mais de 200 caracteres.")]
        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;

        // Relação: Um tipo tem muitas atividades
        public virtual ICollection<Activity> Activities { get; set; }
    }
}