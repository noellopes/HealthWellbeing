using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class MaterialEquipamentoAssociado
    {
        public int MaterialEquipamentoAssociadoId { get; set; }

        [Required(ErrorMessage = "O nome do equipamento é obrigatório.")]
        [StringLength(100)]
        public string NomeEquipamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser um número positivo.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O estado do componente é obrigatório (ex.: operacional, avariado, em manutenção).")]
        [StringLength(50)]
        public string EstadoComponente { get; set; } = string.Empty;

        // NOVO: Propriedade de Navegação para M:N
        public ICollection<ExameTipoRecurso>? ExameTipoRecursos { get; set; }


    }
}
