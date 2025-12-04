using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    // A tabela de junção M:N
    public class ExameTipoRecurso
    {
        // 1. Chave Estrangeira para o ExameTipo (FK 1/2)
        public int ExameTipoId { get; set; }
        public ExameTipo? ExameTipo { get; set; }

        // 2. Chave Estrangeira para o Recurso (FK 2/2)
        public int MaterialEquipamentoAssociadoId { get; set; }
        public MaterialEquipamentoAssociado? Recurso { get; set; }

        //Novo
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser no mínimo 1.")]
        public int QuantidadeNecessaria { get; set; } = 1;

        

        // Esta classe não precisa de uma PK separada (ExameTipoRecursoId), 
        // pois a PK será composta por ExameTipoId + MaterialEquipamentoAssociadoId.
    }
}