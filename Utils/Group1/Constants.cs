using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Models;

namespace HealthWellbeing.Utils.Group1
{
    public class Constants
    {
        private static readonly int DEFAULT_MAX_ITEMS_PER_PAGE = 5;
        public static readonly int NUMBER_PAGES_SHOW_BEFORE_AFTER = 5;

        private static readonly Dictionary<Type, int> _maxItemsPerPage = new() {
            { typeof(TreatmentRecord), 1 },
            { typeof(Pathology), 5 },
            { typeof(TreatmentType), 5 },
        };

        public static readonly AlertItem DEFAULT_NO_CONFIG_ALERT = new AlertItem
        {
            AlertType = "danger",
            IconClass = "bi bi-exclamation-circle",
            Message = "Configuration error: Model type or properties list not provided.",
            Dismissible = false
        };

        public static readonly AlertItem DEFAULT_NO_DATA_ALERT = new AlertItem
        {
            AlertType = "warning",
            IconClass = "bi bi-exclamation-triangle",
            Message = "No records found.",
            Dismissible = false
        };

        public static int MAX_ITEMS_PER_PAGE<T>() => _maxItemsPerPage.TryGetValue(typeof(T), out var value) ? value : DEFAULT_MAX_ITEMS_PER_PAGE;
    }
}
