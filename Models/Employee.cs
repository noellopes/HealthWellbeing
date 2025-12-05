using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please enter the employee {0}.")]
        [StringLength(100, ErrorMessage = "{0} must be at most {1} characters long.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Please enter the employee {0}.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "{0} must be at most {1} characters long.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Please enter the employee {0}.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone")]
        [StringLength(20, ErrorMessage = "{0} must be at most {1} characters long.")]
        public required string PhoneNumber { get; set; }
    }
}
