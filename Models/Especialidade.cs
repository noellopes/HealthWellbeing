using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Especialidade
    {
        [Key]
        public int EspecialidadeId { get; set; }

        [Required]
        [StringLength(40)]
        public string Nome { get; set; } = string.Empty;

        public ICollection<Terapeuta> Terapeutas { get; set; } = new List<Terapeuta>();
    }
}