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

        // 1. Foreign Key Property (FK)
        // This holds the actual ID of the EstadoMaterial (e.g., 1, 2, 3)
        [Required(ErrorMessage = "O estado do componente é obrigatório.")]
        // Make sure this name matches the primary key type (int) in EstadoMaterial
        public int MaterialStatusId { get; set; }

        // 2. Navigation Property
        // This allows you to load the actual EstadoMaterial object (e.g., its Nome, Descricao)
        public EstadoMaterial? EstadoMaterial { get; set; }

        // NOVO: Propriedade de Navegação para M:N
        public ICollection<ExameTipoRecurso>? ExameTipoRecursos { get; set; }


    }
}
