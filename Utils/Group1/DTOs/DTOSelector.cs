using HealthWellbeing.Models;
using System.Linq.Expressions;

namespace HealthWellbeing.Utils.Group1.DTOs
{
    public record DtoSelector<TModel, TDto>(
        Expression<Func<TModel, TDto>> Params,
        IReadOnlyList<string> DisplayFields
    );
}
