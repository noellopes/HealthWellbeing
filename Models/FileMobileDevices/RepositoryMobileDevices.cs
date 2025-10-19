namespace HealthWellbeingRoom.Models.FileMobileDevices
{
    public class RepositoryMobileDevices
    {
        private static List<MobileDevices> dispositivos = new List<MobileDevices>();
        public static IEnumerable<MobileDevices> Index => dispositivos;
        public static void AddMobileDevices(MobileDevices dispositivo) => dispositivos.Add(dispositivo);

        public static void UpdateMobileDevices(MobileDevices dispositivoAtualizado)
        {
            //Encontra o índice do dispositivo original na lista
            var indice = dispositivos.FindIndex(d => d.DevicesID == dispositivoAtualizado.DevicesID);

            //Se for encontrado (indice != -1), substitui o objeto
            if (indice != -1)
            {
                dispositivos[indice] = dispositivoAtualizado;
            }
        }

        public static void DeleteMobileDevices(int id)
        {
            //Encontra o dispositivo com o ID fornecido
            var dispositivo = dispositivos.FirstOrDefault(d => d.DevicesID == id);

            //Se o dispositivo existir, remove-o da lista
            if (dispositivo != null)
            {
                dispositivos.Remove(dispositivo);
            }
        }

    }
}

