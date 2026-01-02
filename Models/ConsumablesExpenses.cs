using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class ConsumablesExpenses
    {
        public int ConsumablesExpensesId { get; set; }

        // Which consumable was used
        public int ConsumableId { get; set; }
        public Consumivel Consumable { get; set; }

        // Where it was used
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Which reservation generated this expense
        public int RoomReservationId { get; set; }
        public RoomReservation RoomReservation { get; set; }

        // How much was consumed
        public int QuantityUsed { get; set; }

        // When it happened
        public DateTime UsedAt { get; set; }
    }
}