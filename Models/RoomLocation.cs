using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HealthWellbeingRoom.Models
{
    public class RoomLocation
    {
        // Identificador único da localização da sala
        [Key]
        public int RoomLocationId { get; set; }

        // Nome da localização
        public string? Name { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
