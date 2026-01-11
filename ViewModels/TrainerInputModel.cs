using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModels
{
    public class TrainerInputModel
    {
        public int? TrainerId { get; set; }

        [Required(ErrorMessage = "Please enter the trainer's name.")]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Please enter the work email.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Please enter the contact phone.")]
        [Phone]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Specify the professional specialty.")]
        public string Specialty { get; set; } = string.Empty;

        public bool IsAdministrator { get; set; } = false;
        public bool IsTrainer { get; set; } = true;
    }
}