using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class SpecialtyRequiredConsumable
    {
        public int SpecialtyRequiredConsumableId { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        public int ConsumivelId { get; set; }
        public Consumivel Consumivel { get; set; }

        public int RequiredQuantity { get; set; } = 1;

    }
}
