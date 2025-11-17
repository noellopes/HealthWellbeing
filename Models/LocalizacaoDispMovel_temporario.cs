using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class LocalizacaoDispMovel_temporario //Para testes do MedicalDevice
    {
        [Key]
        public int Id { get; set; }

        // Chaves Estrangeiras
        public int MedicalDeviceID { get; set; }
        public int RoomId { get; set; }

        // Propriedades de Navegação (Para aceder aos dados da outra tabela)
        public MedicalDevice? MedicalDevice { get; set; }
        public Room? Room { get; set; }

        public bool IsCurrent { get; set; } // Necessário para saber qual é o registo ativo
    }
}
