using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HealthWellbeing.Models
{
    public class ZonaArmazenamento
    {
        [Key]
        public int ZonaId { get; set; }

        [Required(ErrorMessage = "O nome da zona é obrigatório.")]
        [StringLength(100)]
        public string NomeZona { get; set; } = string.Empty;

        // FK para Consumível
        [Required(ErrorMessage = "O consumível é obrigatório.")]
        public int ConsumivelId { get; set; }

        // Navegação - NÃO validar no POST
        [ValidateNever]
        public Consumivel? Consumivel { get; set; }

        // FK para Sala (Room)
        [Required(ErrorMessage = "A sala é obrigatória.")]
        public int RoomId { get; set; }

        // Navegação - NÃO validar no POST
        [ValidateNever]
        public Room? Room { get; set; }

        [Required(ErrorMessage = "A capacidade máxima é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade máxima deve ser pelo menos 1.")]
        public int CapacidadeMaxima { get; set; }

        [Required(ErrorMessage = "A quantidade atual é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade atual não pode ser negativa.")]
        public int QuantidadeAtual { get; set; }

        public bool Ativa { get; set; } = true;
    }
}
