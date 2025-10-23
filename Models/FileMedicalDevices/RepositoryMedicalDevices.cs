namespace HealthWellbeingRoom.Models.FileMedicalDevices
{
    public class RepositoryMedicalDevices
    {
        private static List<MedicalDevices> dispositivos = new List<MedicalDevices>();
        public static IEnumerable<MedicalDevices> Index => dispositivos;
        public static void AddMedicalDevices(MedicalDevices dispositivo) => dispositivos.Add(dispositivo);

        public static void UpdateMedicalDevices(MedicalDevices dispositivoAtualizado)
        {
            //Encontra o índice do dispositivo original na lista
            var indice = dispositivos.FindIndex(d => d.DevicesID == dispositivoAtualizado.DevicesID);

            //Se for encontrado (indice != -1), substitui o objeto
            if (indice != -1)
            {
                dispositivos[indice] = dispositivoAtualizado;
            }
        }

        public static void DeleteMedicalDevices(int id)
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

