using HealthWellbeing.Models;
using System.Linq.Expressions;

namespace HealthWellbeing.Utils.Group1.DTOs
{
    public record DtoSelector(
        Expression<Func<TreatmentRecord, TreatmentRecordListDTO>> Params,
        IReadOnlyList<string> DisplayFields
    );
}
