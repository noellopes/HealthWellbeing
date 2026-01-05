namespace HealthWellbeingRoom.ViewModels
{
    public class RoomPreparationMedicalDevice
    {
        public DateTime DataConsulta { get; set; }
        public string NomeSala { get; set; }
        public string Especialidade { get; set; }
        public string Medico { get; set; }

        // Lista de materiais para esta consulta
        public List<MedicalDeviceStatus> Dispositivo { get; set; } = new List<MedicalDeviceStatus>();

        public DateTime? HoraFim { get; set; } 
    }

    public class MedicalDeviceStatus
    {
        public string NomeDispositivo { get; set; }
        public int QtdNecessaria { get; set; }
        public int QtdNaSala { get; set; }
        public bool EmFalta => QtdNecessaria > QtdNaSala;
        public int QtdFalta => QtdNecessaria - QtdNaSala;
    }
}
