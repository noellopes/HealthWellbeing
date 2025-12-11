using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }

        [Required]
        public string ActivityName { get; set; } = string.Empty;

        [Required]
        public string ActivityType { get; set; } = string.Empty;

        [Required]
        public string ActivityDescription { get; set; } = string.Empty;

        [Required]
        public int ActivityReward { get; set; }

        // Método da classe, conforme o diagrama
        public void obterRecompensas()
        {
            
        }
    }
}
