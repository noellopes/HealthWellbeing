using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class PlanoExercicioExercicio
    {
        public int PlanoExerciciosId { get; set; }
        public PlanoExercicios PlanoExercicios { get; set; }
        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }
        public bool Concluido { get; set; } = false;
        public double? PesoUsado { get; set; }
    }
}