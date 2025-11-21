using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Utils.Group1.Interfaces
{
    public abstract class IBaseEntity<TPrimaryKey> : IEntityId<TPrimaryKey>, ICreatedEntity, IUpdatableEntity, ISoftDeletableEntity
    {
        // Id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TPrimaryKey Id { get; set; }

        // Info fields
        [Display(Name = "Data de submissão")]
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;


        // Audit Fields
        [Display(Name = "Data de modificação")]
        public virtual DateTimeOffset? LastModifiedAt { get; set; }

        [Display(Name = "Modificado por")]
        public virtual String? LastModifiedBy { get; set; }

        // Soft Deletion
        public virtual bool IsDeleted { get; set; }

        [Display(Name = "Data de remoção")]
        public virtual DateTimeOffset? DeletedAt { get; set; }

    }
}
