using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;

namespace HealthWellbeingRoom.ViewModels
{
    public class RoomMaterial
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;

        public List<LocationMedDevice> MedicalDevices { get; set; } = new();
        public List<RoomConsumable> Consumables { get; set; } = new();
    }

}
