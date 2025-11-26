using HealthWellbeing.Utils.Group1.DTOs;

namespace HealthWellbeing.Utils.Group1.Models
{
    public class Group1DataResponseObject<TModel, TDTO>
    {
        public PaginatedList<TDTO> Data { get; private set; }
        public DtoSelector<TModel, TDTO> Selector { get; private set; }
        public Type DTOType => typeof(TDTO);

        public Group1DataResponseObject(PaginatedList<TDTO> data, DtoSelector<TModel, TDTO> selector)
        {
            Data = data;
            Selector = selector;
        }
    }

    public class Group1DataResponseObjectSingle<TModel, TDTO>
    {
        public TDTO Data { get; private set; }
        public DtoSelector<TModel, TDTO> Selector { get; private set; }
        public Type DTOType => typeof(TDTO);

        public Group1DataResponseObjectSingle(TDTO data, DtoSelector<TModel, TDTO> selector)
        {
            Data = data;
            Selector = selector;
        }
    }
}
