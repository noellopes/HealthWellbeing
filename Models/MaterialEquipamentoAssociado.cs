using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class MaterialEquipamentoAssociado
    {
        [Key]
        public int MaterialEquipamentoAssociadoId { get; set; }

        [Required(ErrorMessage = "O nome do material é obrigatório.")]
        [StringLength(100)]
        public string NomeEquipamento { get; set; } = string.Empty;


        [Required(ErrorMessage = "O tamannho é obrigatória.")]
        [StringLength(20)]
        public string? Tamanho { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(0, int.MaxValue)]
        public int Quantidade { get; set; }

        // Navegação: Permite ver em que "Protocolos" este material aparece
        public virtual ICollection<ExameTipoRecurso> ExameTipoRecursos { get; set; } = new List<ExameTipoRecurso>();
    }
}