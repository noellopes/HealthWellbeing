using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}