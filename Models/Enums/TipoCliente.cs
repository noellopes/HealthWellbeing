using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models.Enums
{
    public enum TipoCliente
    {
        [Display(Name = "Normal")]
        Normal = 1,

        [Display(Name = "Sócio")]
        Socio = 2,

        [Display(Name = "VIP")]
        Vip = 3
    }

}
