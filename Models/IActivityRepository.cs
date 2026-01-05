namespace HealthWellbeing.Models
{
    public interface IActivityRepository
    {
        IEnumerable<Activity_> Activities { get; }
    }
}
