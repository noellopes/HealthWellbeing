namespace HealthWellbeing.Models
{
    public class ExercicioGenero
    {
        // Chave Estrangeira para Exercicio
        public int ExercicioId { get; set; }
        public Exercicio? Exercicio { get; set; }

        // Chave Estrangeira para Genero
        public int GeneroId { get; set; }
        public Genero? Genero { get; set; }
    }
}
