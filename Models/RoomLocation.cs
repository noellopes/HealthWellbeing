using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HealthWellbeingRoom.Models
{
    public class RoomLocation
    {
        [Key]
        public int RoomLocationId { get; set; }

        public string? Name { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
