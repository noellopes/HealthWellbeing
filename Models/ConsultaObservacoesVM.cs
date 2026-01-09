using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModel
{
    public class ConsultaObservacoesVM{
        public int IdConsulta { get; set; }

        [Display(Name = "Observações")]
        [MaxLength(4000, ErrorMessage = "As observações não podem ter mais de 4000 caracteres.")]
        public string? Observacoes { get; set; }
    }
}
