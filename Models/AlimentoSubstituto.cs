using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class AlimentoSubstituto
    {
        [Key]
        public int AlimentoSubstitutoId { get; set; }

        // Alimento original
        [Required(ErrorMessage = "O alimento original é obrigatório.")]
        [ForeignKey(nameof(AlimentoOriginal))]
        [Display(Name = "Alimento Original")]
        public int AlimentoOriginalId { get; set; }

        public Alimento? AlimentoOriginal { get; set; }

        // Alimento substituto
        [Required(ErrorMessage = "O alimento substituto é obrigatório.")]
        [ForeignKey(nameof(AlimentoSubstitutoRef))]
        [Display(Name = "Alimento Substituto")]
        public int AlimentoSubstitutoRefId { get; set; }

        public Alimento? AlimentoSubstitutoRef { get; set; }

        // Dados adicionais da substituição
        [StringLength(200, ErrorMessage = "O motivo deve ter no máximo 200 caracteres.")]
        [Display(Name = "Motivo da Substituição")]
        public string? Motivo { get; set; }

        [Range(0.1, 10, ErrorMessage = "A proporção equivalente deve estar entre 0.1 e 10.")]
        [Display(Name = "Proporção Equivalente")]
        public decimal? ProporcaoEquivalente { get; set; } = 1;

        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [Range(0, 1, ErrorMessage = "O fator de similaridade deve estar entre 0 e 1.")]
        [Display(Name = "Fator de Similaridade")]
        [DisplayFormat(DataFormatString = "{0:P0}", ApplyFormatInEditMode = false)]
        public double? FatorSimilaridade { get; set; } = 0.5;
    }
}
