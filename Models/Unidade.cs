using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Unidade
    {
        public int UnidadeId { get; set; }

        [Required, StringLength(60)]
        public string NomeComida { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Quantidade { get; set; } = string.Empty;
    }
}
