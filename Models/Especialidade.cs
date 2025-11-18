using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellBeing.Models
{
    public class Especialidade
    {
        public int EspecialidadeId { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; }

        // Propriedade de Navegação: Uma especialidade pode ter muitos tipos de exame.
        public ICollection<ExameTipo>? TiposExame { get; set; }
    }
}