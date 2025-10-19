using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class User
    {
        [Required(ErrorMessage = "Please enter your ID")]
        public string UserId { get; set; } = default!;

        [Required(ErrorMessage = "Please enter your password")]
        [StringLength(20, MinimumLength = 4)]
        public string Password { get; set; } = default!;

        [Required]
        public string Role { get; set; } = default!; 
    }
}
