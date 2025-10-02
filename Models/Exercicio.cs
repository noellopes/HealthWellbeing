namespace HealthWellbeing.Models
{
    public class Exercicio
    {
        public int ExercicioId { get; set; }

        public string ExercicioName { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public double Duracao { get; set; } = 0;

        public int Intencidade { get; set; } = 0;

        public double CaloriasGastas { get; set; } = 0;
    }
}
