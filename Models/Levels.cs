using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace HealthWellbeing.Models
{
    public class Level
    {
        [Key]
        public int LevelId { get; set; } // Primary Key
        [Range(1, 100, ErrorMessage = "Level must be 1-100")]
        public int LevelNumber { get; set; } // Level number 1-100
        //[ValidateNever]
        //public string LevelPoints { get; set; } // Total points of the level
        [ValidateNever]
        public string LevelCategory { get; set; } // Level Category (If X level then Y category and Z circle color)
        [ValidateNever]
        public string Description { get; set; }  // Description 


        public Level() { }

        public Level(int level)
        {
            LevelNumber = level;
            LevelCategory = GetCircleColor(level);
        }

        public string GetCircleColor(int level)
        {
            if (level < 25)
            {
                Description = "Beginner";
                return "green";
            }
            else if (level < 50)
            {
                Description = "Intermediate";
                return "blue";
            }
            else if (level < 75)
            {
                Description = "Expert";
                return "yellow";
            }
            Description = "Master";
            return "red";
        }

    }
}
