namespace HealthWellbeingRoom.Models
{
    // Classe que representa a relação entre especialidades médicas e dispositivos médicos obrigatórios
    //permite N:N entre Specialties e MedicalDevices
    public class SpecialtyRequiredDevice
    {
        public int SpecialtyRequiredDeviceId { get; set; }

        // Especialidade
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        // Dispositivo obrigatório
        public int MedicalDeviceId { get; set; }
        public MedicalDevice MedicalDevice { get; set; }

        // Opcional: quantidade necessária
        public int RequiredQuantity { get; set; } = 1;
    }
}
