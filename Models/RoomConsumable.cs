using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class RoomConsumable
    {
        [Key]
        public int RoomConsumableId { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [ForeignKey("Consumivel")]
        public int ConsumivelId { get; set; }
        public Consumivel? Consumivel { get; set; }  // <-- Navegação obrigatória

        public int Quantity { get; set; }
        public string? Note { get; set; }

    }
}
