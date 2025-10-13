namespace HealthWellbeing.Models {
    public class EventType {

        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public string EventTypeDescription { get; set; }
        public bool IsPublished { get; set; } = false;

    }
}
