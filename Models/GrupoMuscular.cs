using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class GrupoMuscular
    {

        public int GrupoMuscularId { get; set; } // Chave primária

        [Required(ErrorMessage = "O nome do grupo muscular é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do grupo muscular não pode exceder 100 caracteres.")]
        [Display(Name = "Nome do Grupo Muscular")]
        public string GrupoMuscularNome { get; set; }

        [StringLength(150, ErrorMessage = "A localização corporal não pode exceder 150 caracteres.")]
        [Display(Name = "Localização Corporal")]
        public string? LocalizacaoCorporal { get; set; }
        public ICollection<Musculo>? Musculos { get; set; } = new List<Musculo>();

        public ICollection<Exercicio>? Exercicio { get; set; }
    }
}