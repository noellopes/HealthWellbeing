namespace HealthWellbeing.Models
{
    public class ExercicioTipoExercicio
    {
        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }

        public int TipoExercicioId { get; set; }
        public TipoExercicio TipoExercicio { get; set; }
    }
}
