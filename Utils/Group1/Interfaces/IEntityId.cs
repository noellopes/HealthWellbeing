namespace HealthWellbeing.Utils.Group1.Interfaces
{
    internal interface IEntityId<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
