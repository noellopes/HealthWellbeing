using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Utils.Group1.Interfaces
{
    public interface IUpdatableEntity
    {
        public DateTimeOffset? LastModifiedAt { get; set; }

        public String? LastModifiedBy { get; set; }

    }
}
