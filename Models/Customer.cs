using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Models {
    [Index(nameof(Email), IsUnique = true)]
    public class Customer {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter {0}.")]
        [StringLength(100, ErrorMessage = "{0} must be at most {1} characters long.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Please enter {0}.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "{0} must be at most {1} characters long.")]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone")]
        [StringLength(20, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Registration Date")]
        [Required(ErrorMessage = "Please enter the {0}.")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Please select the {0}.")]
        [StringLength(20, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [RegularExpression("Masculino|Feminino", ErrorMessage = "Invalid gender selected.")]
        public required string Gender { get; set; }

        [Display(Name = "Total Points")]
        [Required(ErrorMessage = "The {0} is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "The {0} cannot be negative.")]
        public int TotalPoints { get; set; } = 0;
    }
}

