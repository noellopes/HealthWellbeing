namespace HealthWellbeing.Models
{
    public class ExercicioProblemaSaude
    {
        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }

        public int ProblemaSaudeId { get; set; }
        public ProblemaSaude ProblemaSaude { get; set; }
    }

}
