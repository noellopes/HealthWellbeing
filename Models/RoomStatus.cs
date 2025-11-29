using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomStatus
    {
        // Identificador único do status da sala
        [Key]
        public int RoomStatusId { get; set; }

        // Nome do status
        public string? Name { get; set; }

        // Descrição do status
        public string? Description { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}