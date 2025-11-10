using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class GrupoMuscular
    {
        public int GrupoMuscularId { get; set; } // ID

        [Required(ErrorMessage = "O nome do grupo muscular é obrigatório.")]
        [StringLength(100)]
        public string GrupoMuscularNome { get; set; } // Nome do grupo Muscular (Peito,Costas,Braços,...)

        [Required(ErrorMessage = "O nome do músculo é obrigatório.")]
        [StringLength(100)]
        public string Musculo { get; set; } // 💪 Nome do músculo específico dentro do grupo Exemplo: "Peitoral Maior", "Dorsal Largo", "Bíceps Braquial"

        [StringLength(150)]
        public string LocalizacaoCorporal { get; set; } // 📍 Localização anatómica do grupo muscular Exemplo: "Parte superior do tronco", "Membros inferiores"

        public ICollection<Exercicio>? Exercicio { get; set; }
    }
}
