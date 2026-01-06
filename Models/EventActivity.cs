namespace HealthWellbeing.Models
{
    public class EventActivity
    {
        public int EventId { get; set; }
        public virtual Event? Event { get; set; }

        public int ActivityId { get; set; }
        public virtual Activity? Activity { get; set; }
    }
}