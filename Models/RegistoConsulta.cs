using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HealthWellbeing.Models
{
    public class RegistoConsulta
    {

        [Required(ErrorMessage = "Please enter the register id!")]
        public int RegisterId { get; set; } = default!;

    }
}
