namespace HealthWellbeing.Models
{
    public class ObjetivoTipoExercicio
    {
        public int ObjetivoFisicoId { get; set; }
        public ObjetivoFisico? ObjetivoFisico { get; set; }

        public int TipoExercicioId { get; set; }
        public TipoExercicio? TipoExercicio { get; set; }
    }
}
