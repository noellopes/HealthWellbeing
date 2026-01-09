namespace HealthWellbeing.Models
{
    public interface IEventRepository
    {
        IEnumerable<Event> Events { get; } 
        Event? GetEventById(int id);       
    }
}
