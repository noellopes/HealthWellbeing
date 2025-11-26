using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace HealthWellbeing.Models
{
    public class Level
    {
        // Primary Key
        public int LevelId { get; set; }



        // Level number 1-100
        [Display(Name = "Level Number")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [Range(1, 100, ErrorMessage = "{0} must be 1-100")]
        public int LevelNumber { get; set; }



        // Level Category
        [Display(Name = "Level Category")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string LevelCategory { get; set; }



        // Level Points Limit
        [Display(Name = "Points Limit")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public int LevelPointsLimit { get; set; }



        // Description 
        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string Description { get; set; }
    }
}
