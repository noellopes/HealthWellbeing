using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class AlimentoSubstituto
    {
        [Key]
        public int AlimentoSubstitutoId { get; set; }

        [Required(ErrorMessage = "O ID do alimento é obrigatório.")]
        [Display(Name = "ID do Alimento Original")]
        public int AlimentoId { get; set; }

        [Required(ErrorMessage = "É necessário informar pelo menos um substituto.")]
        [MinLength(1, ErrorMessage = "Informe pelo menos um substituto válido.")]
        [Display(Name = "IDs dos Alimentos Substitutos")]
        public List<int> SubstitutoIds { get; set; } = new();

        [StringLength(200, ErrorMessage = "O motivo deve ter no máximo 200 caracteres.")]
        [Display(Name = "Motivo da Substituição")]
        public string? Motivo { get; set; }

        [Range(0.1, 10, ErrorMessage = "A proporção equivalente deve estar entre 0.1 e 10.")]
        [Display(Name = "Proporção Equivalente")]
        public decimal ProporcaoEquivalente { get; set; }

        [StringLength(300, ErrorMessage = "As observações devem ter no máximo 300 caracteres.")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }
    }
}
