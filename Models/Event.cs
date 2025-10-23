namespace HealthWellbeing.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Intensity { get; set; } = string.Empty;
        public int CaloriesBurned { get; set; }
        public DateTime EventDate { get; set; } = DateTime.Now;
    }
}
