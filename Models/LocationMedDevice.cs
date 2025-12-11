using HealthWellbeingRoom.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class LocationMedDevice
    {
        [Key]
        [Display(Name = "ID da Localização do Dispositivo Médico")]
        public int LocationMedDeviceID { get; set; }

        [Required(ErrorMessage = "O ID do dispositivo médico é obrigatório.")]
        [Display(Name = "Dispositivo Médico")]
        public int MedicalDeviceID { get; set; }
        public MedicalDevice? MedicalDevice { get; set; } // Propriedade de navegação para MedicalDevice


        [Required(ErrorMessage = "O ID da sala é obrigatório.")]
        [Display(Name = "Sala")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }  // Propriedade de navegação para Room


        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Início")]
        public DateTime InitialDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Fim")]
        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; } // Indica se o dispositivo está atualmente na localização

    }
}
