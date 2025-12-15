using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class RecordConsumablesExpenses
    {
        public int Id { get; set; }
        public int TreatmentSessionId { get; set; }
        public int ConsumableId { get; set; }
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; }
        public int RoomId { get; set; }

        //public TreatmentSession TreatmentSession { get; set; }
        public Consumivel Consumable { get; set; }
        public Room Room { get; set; }

    }
}
