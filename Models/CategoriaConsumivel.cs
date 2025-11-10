using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class CategoriaConsumivel
    {
        [Key]
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}