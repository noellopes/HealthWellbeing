namespace HealthWellbeingRoom.Models
{
    public class EquipmentType
    {
        public int EquipmentTypeId { get; set; }

        public required string Name { get; set; }

        public int ManufacturerId { get; set; }

        public Manufacturer? Manufacturer { get; set; }
    }
}
