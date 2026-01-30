using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class NivelCliente
    {
        [Key]
        public int NivelClienteId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        public int PontosMinimos { get; set; }

        [Required]
        public string CorBadge { get; set; } // ex: bg-secondary, bg-warning
    }
}
