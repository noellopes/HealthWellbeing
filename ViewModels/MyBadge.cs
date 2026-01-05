using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels {
    public class MyBadge {
        public required Badge Badge { get; set; }
        public bool IsEarned { get; set; }
        public DateTime? DateAwarded { get; set; }
    }
}
