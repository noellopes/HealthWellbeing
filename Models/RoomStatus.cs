using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomStatus
    {
        [Key]
        public int RoomStatusId { get; set; }
        public required string Name { get; set; }//precisa permitir nulos

        public string? Description { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}