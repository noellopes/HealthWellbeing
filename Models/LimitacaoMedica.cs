using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class LimitacaoMedica
    {
        public int LimitacaoMedicaId { get; set; } // Chave primária

        [Required(ErrorMessage = "A descrição da limitação médica é obrigatória.")]
        [StringLength(300, ErrorMessage = "A descrição da limitação médica não pode exceder 300 caracteres.")]
        [Display(Name = "Limitação Médica")]
        public string Descricao { get; set; }

        [StringLength(150, ErrorMessage = "O tipo de limitação não pode exceder 150 caracteres.")]
        [Display(Name = "Tipo de Limitação")]
        public string? TipoLimitacao { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        public string? Observacoes { get; set; }
    }
}