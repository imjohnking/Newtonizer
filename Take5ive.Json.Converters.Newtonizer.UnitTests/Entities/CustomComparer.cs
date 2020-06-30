using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using static Take5ive.Json.Converters.UnitTests.Helpers;

namespace Take5ive.Json.Converters.UnitTests.Entities
{
    public class CustomComparer<T> : IEqualityComparer<T>
    {
        JsonSerializerOptions _jsonSerializerOptions;

        public CustomComparer(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public bool Equals(T expected, T actual)
        {
            return IsEqual(expected, actual);
        }

        public int GetHashCode(T parameterValue)
        {
            return Tuple.Create(parameterValue).GetHashCode();
        }

        private bool IsEqual(object expected, object actual)
        {
            var props = expected.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var prop in props)
            {
                var expectedValue = prop.GetValue(expected);
                var actualValue = prop.GetValue(actual);

                // if we should ignore this property, skip it
                if (IgnoreProperty(prop, _jsonSerializerOptions))
                {
                    continue;
                }

                // recursively call this function to process nested objects
                if (prop.PropertyType.IsNested)
                {
                    if (!IsEqual(expectedValue, actualValue))
                    {
                        return false;
                    }
                    continue;
                }

                // if both values are null then they match
                if (expectedValue == null && actualValue == null)
                {
                    continue;
                }

                // we know that both are not null so if either is null they don't match
                if (expectedValue == null || actualValue == null)
                {
                    return false;
                }

                // if the property is an array, compare values accordingly
                // NOTE: if the array is not string[] or int[] this will not work...
                // may want to serialize both arrays for comparison in that case...
                if (prop.PropertyType.IsArray)
                {
                    if (prop.PropertyType == typeof(string[]) && !Enumerable.SequenceEqual((string[])expectedValue, (string[])actualValue))
                    {
                        return false;
                    }

                    if (prop.PropertyType == typeof(int[]) && !Enumerable.SequenceEqual((int[])expectedValue, (int[])actualValue))
                    {
                        return false;
                    }

                    continue;
                }

                // due to the way System.Text.Json.JsonSerializer.Deserialize handles extension data
                // the easiest way to perform this compare is to serialize expected and actual and compare them
                if (prop.GetCustomAttribute<System.Text.Json.Serialization.JsonExtensionDataAttribute>() != null)
                {
                    if (JsonSerializer.Serialize(expectedValue) == JsonSerializer.Serialize(actualValue))
                    {
                        // values match, move to next
                        continue;
                    }
                    else
                    {
                        // values do not match
                        return false;
                    }
                }

                // now that we know neither is null we can perform the value comparison
                if (!expectedValue.Equals(actualValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
