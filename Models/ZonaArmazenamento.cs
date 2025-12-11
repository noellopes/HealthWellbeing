using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ZonaArmazenamento
    {
        [Key]
        public int ZonaId { get; set; }

        // FK -> Consumível (um tipo de consumível por zona)
        [Required]
        [Display(Name = "Consumível")]
        public int ConsumivelId { get; set; }
        public Consumivel Consumivel { get; set; }

        // FK -> Sala (Room do outro grupo)
        [Required]
        [Display(Name = "Sala")]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Required(ErrorMessage = "O nome da zona é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da zona não pode ter mais de 100 caracteres.")]
        [Display(Name = "Nome da Zona")]
        public string NomeZona { get; set; }

        [Required(ErrorMessage = "A capacidade máxima é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade máxima deve ser pelo menos 1 unidade.")]
        [Display(Name = "Capacidade Máxima (unidades)")]
        public int CapacidadeMaxima { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade atual não pode ser negativa.")]
        [Display(Name = "Quantidade Atual (unidades)")]
        public int QuantidadeAtual { get; set; }

        [Display(Name = "Zona Ativa")]
        public bool Ativa { get; set; } = true;
    }
}
