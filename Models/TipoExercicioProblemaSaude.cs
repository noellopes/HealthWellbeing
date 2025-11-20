namespace HealthWellbeing.Models
{
    public class TipoExercicioProblemaSaude
    {
        public int TipoExercicioId { get; set; }
        public TipoExercicio? TipoExercicio { get; set; }

        public int ProblemaSaudeId { get; set; }
        public ProblemaSaude? ProblemaSaude { get; set; }
    }
}
