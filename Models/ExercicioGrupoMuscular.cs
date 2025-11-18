namespace HealthWellbeing.Models
{
    public class ExercicioGrupoMuscular
    {
        // Chave Estrangeira para Exercicio
        public int ExercicioId { get; set; }
        public Exercicio? Exercicio { get; set; }

        // Chave Estrangeira para GrupoMuscular
        public int GrupoMuscularId { get; set; }
        public GrupoMuscular? GrupoMuscular { get; set; }
    }
}
