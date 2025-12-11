/*
* THIS CLASS AND CODE WAS CREATED BY GROUP 1 FOR SHARED REUSABILITY
* You can use this functions in your own views, etc...
* Don't Modify, any changes or additions. open a pull request to our branch (Group-1-MAIN)
*/

using HealthWellbeing.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace HealthWellbeing.Utils.Group1
{
    public static class Functions
    {
        /// <summary>
        /// Performs a hard delete (direct SQL DELETE) on a record of the specified entity type by its ID.
        /// This method bypasses EF Core's change tracking and interceptors, ensuring the record is permanently removed from the database.
        /// It includes validation for entity existence, table name resolution, and uses a transaction for atomicity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to delete, must be a class and part of the DbContext model.</typeparam>
        /// <param name="context">The DbContext instance to use for the operation.</param>
        /// <param name="id">The ID of the record to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown if the entity type is not part of the model or if the table name cannot be resolved.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no record with the specified ID exists.</exception>
        /// <remarks>
        /// This method is intended for administrative use only, as it permanently deletes data from the database.
        /// Ensure proper authorization checks are in place before calling this method.
        /// </remarks>
        public static async Task HardDeleteByIdAsync<TEntity>(HealthWellbeingDbContext _context, int id) where TEntity : class
        {
            // Validate entity type and table name
            var entityType = _context.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
                throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not part of the model.");

            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException("Unable to resolve table name for entity.");

            // Check if entity exists before deletion
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Entity not found.");

            // Build parameterized SQL to prevent injection
            var sql = $"DELETE FROM [{tableName}] WHERE Id = @id";
            var parameter = new SqlParameter("@id", id);

            // Execute in a transaction for atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql, parameter);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public static string GetEnumDisplayName(Enum value)
        {
            var memberInfo = value.GetType().GetMember(value.ToString());
            if (memberInfo.Length > 0)
            {
                var displayAttr = memberInfo[0]
                    .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                    .FirstOrDefault() as System.ComponentModel.DataAnnotations.DisplayAttribute;
                if (displayAttr != null)
                    return displayAttr.Name;
            }
            return value.ToString();
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
            if (obj == null || string.IsNullOrEmpty(propertyPath)) return null;

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
