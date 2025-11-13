using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {

    public class EventType {
        public int EventTypeId { get; set; }

        [Required, StringLength(200)]
        public string EventTypeName { get; set; }

        [Required, StringLength(50)]
        public string EventTypeScoringMode { get; set; }

        public float EventTypeMultiplier { get; set; }

    }
}
