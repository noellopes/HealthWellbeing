namespace HealthWellbeing.Models {
    public interface IEventTypeRepository {
        IEnumerable<EventType> EventTypes { get; }
    }
}
