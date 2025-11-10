using System.ComponentModel.DataAnnotations; // <--- Não se esqueça disto!

namespace HealthWellbeing.Models
{
    public class CategoriaConsumivel
    {
        [Key] // <--- Indica explicitamente que esta é a Primary Key
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}