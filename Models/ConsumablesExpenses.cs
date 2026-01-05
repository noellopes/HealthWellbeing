using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class ConsumablesExpenses
    {
        public int ConsumablesExpensesId { get; set; }

        // Qual consumível foi utilizado
        public int ConsumableId { get; set; }
        public Consumivel Consumable { get; set; }

        // Onde foi utilizado
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Qual reserva gerou esta despesa
        public int? RoomReservationId { get; set; }
        public RoomReservation? RoomReservation { get; set; }

        // Quantidade consumida
        public int QuantityUsed { get; set; }

        // Quando ocorreu
        public DateTime UsedAt { get; set; }
    }
}