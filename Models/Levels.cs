namespace HealthWellbeing.Models
{
    public class Levels
    {
        public int LevelId { get; set; } // Primary Key
        public int Level { get; set; } // Level number 1-100
        public string LevelCategory { get; set; } // Level Category (<=25 beginner, <=50 intermediate,<=100 master)
        public string Description { get; set; }=default!;  // Description 

    }
}
