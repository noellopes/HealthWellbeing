using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomConsumable
    {
        [Key]
        public int RoomConsumableId { get; set; }

        // FK para Room
        [Required]
        public int RoomId { get; set; }

        // Navegação para Room (opcional para evitar warnings; EF irá popular quando incluído)
        public Room? Room { get; set; }

        // FK para Consumivel
        [Required]
        public int ConsumivelId { get; set; }

        // Navegação para Consumivel (opcional para evitar warnings; torna não anulável se a relação for sempre obrigatória)
        public Consumivel? Consumivel { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um número não negativo.")]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
    }
}