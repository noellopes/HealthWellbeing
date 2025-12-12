using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class RoomConsumable
    {
        public int RoomConsumableId { get; set; }

        // Nome do consumível
        public string Name { get; set; }

        public string Category { get; set; }

        // Quantidade associada
        public int Quantity { get; set; }

        // Observações ou notas (opcional)
        public string? Note { get; set; }
    }
}
