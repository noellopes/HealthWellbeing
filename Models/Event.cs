namespace HealthWellbeing.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;

        public string EventPoints {  get; set; }


    }
}
