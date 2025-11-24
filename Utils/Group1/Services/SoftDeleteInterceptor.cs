using HealthWellbeing.Utils.Group1.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HealthWellbeing.Utils.Group1.Services
{
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
            {
                return base.SavingChangesAsync(
                    eventData, result, cancellationToken);
            }

            IEnumerable<EntityEntry<ISoftDeletable>> entries =
            eventData
                .Context
                .ChangeTracker
                .Entries<ISoftDeletable>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (EntityEntry<ISoftDeletable> softDeletable in entries)
            {
                softDeletable.State = EntityState.Modified;
                softDeletable.Entity.IsDeleted = true;
                softDeletable.Entity.DeletedAt = DateTimeOffset.UtcNow;
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
