using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class RestricaoAlimentarAlimento
    {
        public int RestricaoAlimentarId { get; set; }
        public RestricaoAlimentar RestricaoAlimentar { get; set; }

        public int AlimentoId { get; set; }
        public Alimento Alimento { get; set; }
    }
}
