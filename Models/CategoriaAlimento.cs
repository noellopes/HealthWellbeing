using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class CategoriaAlimento
    {
        [Key]
        public int CategoriaID { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; }
    }
}
