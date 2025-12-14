using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exercicio
    {
        public int ExercicioId { get; set; }

        [Required(ErrorMessage = "O nome do exercício é obrigatório.")]
        [StringLength(100)]
        public string ExercicioNome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O Tipo de Exercício é obrigatório.")]
        public int TipoExercicioId { get; set; }
        public TipoExercicio TipoExercicio { get; set; }

        [Required(ErrorMessage = "A duração é obrigatória.")]
        [Range(0.1, 480, ErrorMessage = "A duração deve ser entre 0.1 e 480 minutos.")]
        public double Duracao { get; set; }

        [Required(ErrorMessage = "A intensidade é obrigatória.")]
        [Range(1, 10, ErrorMessage = "A intensidade deve ser entre 1 e 10.")]
        public int Intencidade { get; set; }

        [Required(ErrorMessage = "As calorias gastas são obrigatórias.")]
        [Range(0.1, 2000, ErrorMessage = "As calorias devem ser entre 0.1 e 2000.")]
        public double CaloriasGastas { get; set; }

        [Required(ErrorMessage = "As instruções são obrigatórias.")]
        public string Instrucoes { get; set; }

        [Required(ErrorMessage = "As repetições são obrigatórias.")]
        [Range(1, 1000, ErrorMessage = "As repetições devem ser entre 1 e 1000.")]
        public int Repeticoes { get; set; }

        [Required(ErrorMessage = "As séries são obrigatórias.")]
        [Range(1, 100, ErrorMessage = "As séries devem ser entre 1 e 100.")]
        public int Series { get; set; }

        public ICollection<ExercicioGenero>? ExercicioGeneros { get; set; }
        public ICollection<ExercicioGrupoMuscular>? ExercicioGrupoMusculares { get; set; }
        public ICollection<ExercicioEquipamento>? ExercicioEquipamentos { get; set; }
        public ICollection<ExercicioProblemaSaude> Contraindicacoes { get; set; }
        public ICollection<HistoricoAtividade> HistoricoAtividades { get; set; }

        public ICollection<PlanoExercicios>? PlanoExercicios { get; set; }


    }
}
