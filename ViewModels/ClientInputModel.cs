using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModels
{
    public class ClientInputModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100, MinimumLength = 6)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A phone number is required.")]
        [Phone]
        public required string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Birth date is mandatory.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [PasswordPropertyText]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm password")]
        [PasswordPropertyText]
        public required string ConfirmPassword { get; set; }
    }
}