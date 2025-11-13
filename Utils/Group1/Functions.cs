/*
* THIS CLASS AND CODE WAS CREATED BY GROUP 1 FOR SHARED REUSABILITY
* You can use this functions in your own views, etc...
* Don't Modify, any changes or additions. open a pull request to our branch (Group-1-MAIN)
*/

using System.Dynamic;

namespace HealthWellbeing.Utils.Group1
{
    public static class Functions
    {
        private static readonly HashSet<string> ValidAlertTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "primary", "secondary", "success", "danger", "warning", "info", "light", "dark"
        };

        /// <summary>
        /// Returns a valid Bootstrap alert type. If the input alertType is invalid,
        /// it falls back to "info".
        /// </summary>
        /// <param name="alertType">The input alert type string</param>
        /// <returns>Valid alert type string</returns>
        public static string ValidateAlertType(string alertType)
        {
            if (string.IsNullOrEmpty(alertType))
                return "info";

            if (ValidAlertTypes.Contains(alertType))
                return alertType.ToLower();

            return "info";
        }

        /// <summary>
        /// Gets the display name of a property from the specified model type,
        /// using Display or DisplayName attributes if present.
        /// </summary>
        /// Example usage in a controller:
        /// ViewBag.ModelType = typeof(TreatmentType);
        /// ViewBag.Properties = new List&lt;string&gt; { "Name", "Description", "EstimatedDuration", "Priority" };
        /// return View("~/Views/Shared/Group1/_ListViewLayout.cshtml", await _context.TreatmentType.ToListAsync());
        ///
        /// // Nested example
        /// ViewBag.ModelType = typeof(TreatmentRecord);
        /// ViewBag.Properties = new List&lt;string&gt; { "Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status", "CreatedAt" };
        /// return View("~/Views/Shared/Group1/_ListViewLayout.cshtml", await healthWellbeingDbContext.ToListAsync());
        /// <param name="modelType">Type of the model</param>
        /// <param name="propertyName">Property name (top-level only)</param>
        /// <returns>Display name or original property name if no attribute found</returns>
        public static string GetDisplayName(Type modelType, string propertyName)
        {
            var propInfo = modelType.GetProperty(propertyName);
            if (propInfo == null) return propertyName;

            var displayAttr = propInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), true)
                                      .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                                      .FirstOrDefault();

            if (displayAttr != null) return displayAttr.Name;

            var displayNameAttr = propInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true)
                                          .Cast<System.ComponentModel.DisplayNameAttribute>()
                                          .FirstOrDefault();

            if (displayNameAttr != null) return displayNameAttr.DisplayName;

            return propInfo.Name;
        }

        /// <summary>
        /// Resolves and returns the value of a nested property on an object, 
        /// given a dot-separated property path (e.g. "Nurse.Name").
        /// </summary>
        /// Usage example in a Razor view:
        /// @HealthWellbeing.Views.Shared.Group1.Functions.GetNestedPropertyValue(item, prop)
        /// <param name="obj">The object instance</param>
        /// <param name="propertyPath">Dot-separated property path</param>
        /// <returns>The nested property value or null if not found</returns>
        public static object GetNestedPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
                return null;

            var currentObject = obj;

            foreach (var propName in propertyPath.Split('.'))
            {
                if (currentObject == null) return null;

                // Handling for DynamicObject
                if (currentObject is IDynamicMetaObjectProvider)
                {
                    var dictionary = currentObject as IDictionary<string, object>;
                    if (dictionary != null && dictionary.ContainsKey(propName))
                    {
                        currentObject = dictionary[propName];
                    }
                    else
                    {
                        // fallback to DynamicObject's GetMember
                        var dyn = currentObject as dynamic;
                        try
                        {
                            currentObject = dyn.GetType().GetProperty(propName)?.GetValue(dyn);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    var propInfo = currentObject.GetType().GetProperty(propName);
                    if (propInfo == null) return null;
                    currentObject = propInfo.GetValue(currentObject);
                }
            }
            return currentObject;
        }

    }
}
