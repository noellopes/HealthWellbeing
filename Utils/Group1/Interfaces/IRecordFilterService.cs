namespace HealthWellbeing.Utils.Group1.Interfaces
{
    public interface IRecordFilterService<T>
    {
        IReadOnlyList<string> SearchableProperties { get; }

        IQueryable<T> ApplyFilter(IQueryable<T> query, string searchBy, string searchString);

        IQueryable<T> ApplySorting(IQueryable<T> query, string sortOrder);
    }
}
