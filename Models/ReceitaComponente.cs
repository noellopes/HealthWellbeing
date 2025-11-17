using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    /// <summary>
    /// Junction table for N:N relationship between Receita and ComponenteReceita
    /// </summary>
    public class ReceitaComponente
    {
        public int ReceitaId { get; set; }
        public Receita? Receita { get; set; }

        public int ComponenteReceitaId { get; set; }
        public ComponenteReceita? ComponenteReceita { get; set; }
    }
}
