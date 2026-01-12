using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthWellbeing.Models;
using HealthWellBeing.Models;

namespace HealthWellbeing.ViewModels // Nota: Se isto for para a BD, devia estar em .Models
{
    public class RegistoMateriais
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Tamanho { get; set; }
        public int Quantidade { get; set; }

        // --- ADICIONA ESTA LINHA ---
        public int MaterialStatusId { get; set; }
        // ---------------------------

        [ForeignKey("MaterialStatusId")]
        public virtual EstadoMaterial? Estado { get; set; }

        [Required]
        public int ExameId { get; set; }
        [ForeignKey("ExameId")]
        public virtual Exame? Exame { get; set; }
    }
}