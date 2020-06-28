using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Take5ive.Json.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters
{
    internal static class Helpers
    {
        /// <summary>
        /// Check to see if property has a JsonProperty attribute to include null values
        /// </summary>
        /// <param name="property">Property to check</param>
        /// <returns>Boolean decision</returns>
        public static bool HasAllowNullAttribute(PropertyInfo property)
        {
            return property.GetCustomAttribute<JsonPropertyAttribute>()?.NullValueHandling == NullValueHandling.Include;
        }

        /// <summary>
        /// Check to see if property is scoped "internal" or "internal protected".
        /// </summary>
        /// <param name="property">Property to check</param>
        /// <returns>Boolean decision</returns>
        public static bool PropertyIsInternal(PropertyInfo property)
        {
            return property.GetGetMethod(true)?.IsAssembly == true || property.GetGetMethod(true)?.IsFamilyOrAssembly == true;
        }

        /// <summary>
        /// Get a property formatted property name.
        /// </summary>
        /// <param name="property">Property to get name from.</param>
        /// <param name="options">Serializer options to use.</param>
        /// <returns>Will check JsonPropertyNameAttribute value and use that if available.</returns>
        public static string GetPropertyName(PropertyInfo property, JsonSerializerOptions options)
        {
            // get JsonPropertyNameAttribute value if it exists
            string propertyNameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;

            // if a JsonPropertyNameAttribute exists, use its value instead of the actual property name
            var propertyName = !String.IsNullOrEmpty(propertyNameAttribute) ? propertyNameAttribute : property.Name;

            // if a property naming policy exists, use it
            if (options.PropertyNamingPolicy != null)
            {
                propertyName = options.PropertyNamingPolicy.ConvertName(propertyName);
            }

            return propertyName;
        }

        /// <summary>
        /// Get a properly formatted property name based on JsonSerializerOptions naming policy.
        /// </summary>
        /// <param name="propertyName">Property name to use.</param>
        /// <param name="options">Serializer options to use.</param>
        /// <returns>Property name after applying naming policy if it exists.</returns>
        public static string GetPropertyName(string propertyName, JsonSerializerOptions options)
        {
            if (options.PropertyNamingPolicy != null)
            {
                // make sure we are following the naming policy for property names
                propertyName = options.PropertyNamingPolicy.ConvertName(propertyName);
            }

            return propertyName;
        }

        public static bool IgnoreProperty(PropertyInfo property, JsonSerializerOptions options)
        {
            // check to see if the property is readonly and JsonSerializerOptions is set to ignore readonly
            if (!property.CanWrite && options.IgnoreReadOnlyProperties)
            {
                // ignore property
                return true;
            }

            // if the property has an ignore attribute, do not include it
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                // ignore property
                return true;
            }

            // check to see if property is internal
            if (PropertyIsInternal(property))
            {
                // if it is internal, check to see if it has a JsonPropertyName attribute 
                // indicating to include it in the serialization of the object
                if (property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Length > 0 == false)
                {
                    // ignore property
                    return true;
                }
            }

            return false;
        }
    }
}
