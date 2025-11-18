using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomLocation
    {
        [Key]
        public int RoomLocationId { get; set; }

        [Required(ErrorMessage = "O nome da localização é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da localização não pode exceder 100 caracteres.")]
        public string Name { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
