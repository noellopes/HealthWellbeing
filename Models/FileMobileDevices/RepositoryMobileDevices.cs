namespace HealthWellbeingRoom.Models.FileMobileDevices
{
    public class RepositoryMobileDevices
    {
        private static List<MobileDevices> dispositivos = new List<MobileDevices>();
        public static IEnumerable<MobileDevices> Index => dispositivos;
        public static void AddMobileDevices(MobileDevices dispositivo) => dispositivos.Add(dispositivo);

    }
}

