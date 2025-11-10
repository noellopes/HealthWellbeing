using System;
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

        [Required(ErrorMessage = "O ID da sala é obrigatório.")]
        [Display(Name = "Sala")]
        public int RoomID { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Início")]
        public DateTime InitialDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Fim")]
        public DateTime? EndDate { get; set; }
    }
}
