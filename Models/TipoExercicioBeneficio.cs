namespace HealthWellbeing.Models
{
    public class TipoExercicioBeneficio
    {
        public int TipoExercicioId { get; set; }
        public TipoExercicio? TipoExercicio { get; set; }

        public int BeneficioId { get; set; }
        public Beneficio? Beneficio { get; set; }
    }
}
