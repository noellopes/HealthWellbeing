namespace HealthWellbeing.Models {
    public class EventTypeEFRepository: IEventTypeRepository {
        private EventTypeDbContext _dbcontext;

        public EventTypeEFRepository(EventTypeDbContext dbcontext) {
            _dbcontext = dbcontext;
        }

        public IEnumerable<EventType> EventTypes => _dbcontext.EventTypes;
    }
}
