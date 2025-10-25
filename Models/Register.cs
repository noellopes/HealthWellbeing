using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HealthWellbeing.Models
{
    public class Register
    {

        [Required(ErrorMessage = "Please enter the register id!")]
        public int registerId { get; set; } = default!;

    }
}
