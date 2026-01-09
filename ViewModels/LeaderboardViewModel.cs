namespace HealthWellbeing.ViewModels
{
    public class LeaderboardItemViewModel
    {
        public int Rank { get; set; }
        public string CustomerName { get; set; }
        public string LevelName { get; set; }
        public int LevelNumber { get; set; }
        public int TotalPoints { get; set; }
        public int BadgesCount { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}