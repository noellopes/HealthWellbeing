namespace HealthWellbeing.Models
{
    public class AlergiaAlimento
    {

        public int AlergiaId { get; set; } 

        public Alergia? Alergia { get; set; }

        public int AlimentoId { get; set; }

        public Alimento? Alimento { get; set; }
    }
}
