using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomType
    {
        // Identificador único do tipo de sala
        [Key]
        public int RoomTypeId { get; set; }

        // Nome do tipo de sala
        public string? Name { get; set; }

        // Descrição do tipo de sala
        public string? Description { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}