using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;

namespace HealthWellbeingRoom.ViewModels
{
    public class RoomMaterial
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;

        public IEnumerable<LocationMedDevice> MedicalDevices { get; set; } = Enumerable.Empty<LocationMedDevice>();
        public IEnumerable<RoomConsumable> Consumables { get; set; } = Enumerable.Empty<RoomConsumable>();
    }

}
