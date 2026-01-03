using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class RoomConsumable
    {
        [Key]
        public int RoomConsumableId { get; set; }

        // FK para Room
        [Required]
        [Display(Name = "Sala")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        // FK para Consumível
        [Required]
        [Display(Name = "Consumível")]
        public int ConsumivelId { get; set; }
        public Consumivel? Consumivel { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um número não negativo.")]
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; }

        [NotMapped]
        public int usedQuantity { get; set; } = 1;

        [StringLength(500)]
        [Display(Name = "Observações")]
        public string? Note { get; set; }

        // Opcional: para registar quando este stock foi criado/atualizado
        [DataType(DataType.Date)]
        [Display(Name = "Data de Início")]
        public DateTime InitialDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Data de Fim")]
        public DateTime? EndDate { get; set; }

        // Se quiseres marcar o registo atual (sem histórico basta manter sempre true)
        public bool IsCurrent { get; set; } = true;
    }
}
