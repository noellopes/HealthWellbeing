using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    [Index(nameof(SerialNumber), IsUnique = true)]//O serial number vai ser único
    [Table("MedicalDevices")]
    public class MedicalDevice
    {
        //ID dos dispositivos móveis
        [Display(Name = "ID do dispositivo")]
        [Key]
        public int MedicalDeviceID { get; set; }
        

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 100 no max.")]
        public string Name { get; set; }


        [Display(Name = "Número de Série")]
        [Required(ErrorMessage = "O Número de Série é obrigatório.")]
        [StringLength(50, ErrorMessage = "O Número de Série não deve exceder 50 caracteres.")]
        public string SerialNumber { get; set; }

        [Display(Name = "Data de Registo")]
        public DateTime RegistrationDate { get; set; }


        [Display(Name = "Observação")]
        public string? Observation { get; set; }


        //FK da tabela tipo de dispositivo
        [Display(Name = "Tipo de Dispositivo")]
        [Required(ErrorMessage = "O Tipo de Dispositivo é Obrigatório.")]
        public int TypeMaterialID { get; set; }
        public TypeMaterial? TypeMaterial { get; set; }

        //Coleção para a tabela intermediária
        public ICollection<LocalizacaoDispMovel_temporario> LocalizacaoDispMedicoMovel { get; set; } = new List<LocalizacaoDispMovel_temporario>();

        // Status: Em manutenção (Propriedade de controlo)
        public bool IsUnderMaintenance { get; set; } = false;

        // Calcular o estado 
        [Display(Name = "Estado Atual")]
        public string CurrentStatus
        {
            get
            {
                // 1. Prioridade Máxima: Manutenção
                if (IsUnderMaintenance)
                {
                    return "Em Manutenção";
                }

                // 2. Segunda Prioridade: Alocação (Indisponível)
                if (LocalizacaoDispMedicoMovel.Any())
                {
                    return "Indisponível (Alocado)";
                }

                // 3. Última Prioridade: Disponível
                return "Disponível";
            }
        }
    }
}

