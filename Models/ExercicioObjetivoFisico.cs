namespace HealthWellbeing.Models
{
    public class ExercicioObjetivoFisico
    {
        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }

        public int ObjetivoFisicoId { get; set; }
        public ObjetivoFisico ObjetivoFisico { get; set; }
    }
}
