using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HealthWellbeing.Models
{
    public class Register
    {
        
        [Required(ErrorMessage = "Please enter the register id!")]
        public int registerId { get; set; }
        public int pacientId { get; set; }
        public int doctorId { get; set; }
        public string date { get; set; }
        public string diagnostic { get; set; }
        public string observations { get; set; }

    }
}
