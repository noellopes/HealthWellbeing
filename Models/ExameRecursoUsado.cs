using HealthWellBeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class ExameRecursoUsado
    {
        [Key]
        public int Id { get; set; }

        public int ExameId { get; set; }
        [ForeignKey("ExameId")]
        public Exame? Exame { get; set; }

        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public MaterialEquipamentoAssociado? Material { get; set; }

        public int Quantidade { get; set; }
    }
}