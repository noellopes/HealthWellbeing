namespace HealthWellbeing.Models
{
    public class Levels
    {
        public int LevelId { get; set; } // Primary Key
        public int Level { get; set; } // Level number 1-100
        public string LevelCategory { get; set; } // Level Category (If X level then Y category and Z circle color)
        public string Description { get; set; }=default!;  // Description 


        public Levels(int level)
        {
            Level = level;
            LevelCategory = GetCircleColor(level);
        }

        private string GetCircleColor(int level)
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
