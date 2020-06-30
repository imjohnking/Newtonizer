using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Take5ive.Json.Converters.Helpers;

namespace Take5ive.Json.Converters
{
    public partial class Newtonizer : JsonConverterFactory
    {
        /// <summary>
        /// Handles all Read/Write functions for the converter
        /// </summary>
        /// <typeparam name="TValue">Object being serialized/deserialized.</typeparam>
        private class NewtonizerInner<TValue> : JsonConverter<TValue>
        {
            public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Invalid JSON String");
                }

                // create an instance of the return object
                var deserialized = Activator.CreateInstance(typeToConvert);

                // get the properties of the destination object
                PropertyInfo[] properties = deserialized.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // start deserializing
                DeserializeProperties(properties, ref reader, ref deserialized, options);

                // return the finished object
                return (TValue)deserialized;
            }

            private void DeserializeProperties(PropertyInfo[] properties, ref Utf8JsonReader reader, ref object deserialized, JsonSerializerOptions options)
            {
                PropertyInfo property;
                string propertyName;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        // nothing else to deserialize
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        // something is wrong with the input string
                        throw new JsonException($"Expected token type to be PropertyName name instead of {reader.TokenType}");
                    }

                    propertyName = reader.GetString();

                    try
                    {
                        property = properties.Where(p => p.Name.ToLower() == propertyName.ToLower() || p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.ToLower() == propertyName.ToLower()).First();
                    }
                    catch
                    {
                        // There is not a 1:1 match for the data read from the json object and the properties
                        // of the destination object.  Try to put the data in an extension data dictionary.
                        ReadExtensionData(propertyName, properties, ref deserialized, ref reader, options);

                        // move to the next read
                        continue;
                    }

                    // check to see if we should ignore this property
                    if (IgnoreProperty(property, options))
                    {
                        // force a read to get the value off the reader
                        reader.Read();

                        // do nothing with the value and continue
                        continue;
                    }

                    // if property is a nested object
                    if (property.PropertyType.IsNested)
                    {
                        // get all of the possible properties of the destination object we can use
                        PropertyInfo[] props = property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        // create a new instance of the nested object type
                        var newObj = Activator.CreateInstance(property.PropertyType);

                        // force the reader to step into this nested object
                        reader.Read();

                        // recursively call DeserializeProperties to fill in the nested object properties
                        DeserializeProperties(props, ref reader, ref newObj, options);

                        // set the value of the nested object in the parent object
                        property.SetValue(deserialized, newObj);

                        // move to next read
                        continue;
                    }

                    // set the value of the property in the object
                    property.SetValue(deserialized, JsonSerializer.Deserialize(ref reader, property.PropertyType, options));
                }
            }

            /// <summary>
            /// Read unexpected data and try to write it to a dictionary with the JsonExtensionDataAttribute.
            /// </summary>
            /// <param name="propertyName">Name of property read from Json object.</param>
            /// <param name="properties">Properties of the destination object.</param>
            /// <param name="deserializedObject">Destination object after deserialization.</param>
            /// <param name="reader">Json Reader</param>
            /// <param name="options">Json Serialization Options</param>
            /// <remarks>If this code fails, it should mean there was no extension data dictionary to accept the data.</remarks>
            private void ReadExtensionData(string propertyName, PropertyInfo[] properties, ref object deserializedObject, ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                // get the value from the reader
                // this needs to be done regardless of what happens below
                JsonElement json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);

                PropertyInfo property = null;

                try
                {
                    // check to see if a dictionary exists with the JsonExtensionDataAttribute
                    property = properties.Where(p => p.GetCustomAttribute<JsonExtensionDataAttribute>() != null).First();
                }
                catch { }

                if (property != null)
                {
                    // extension data dictionary can be Dictionary<string, JsonElement> or Dictionary<string, object>
                    // make sure we are acting appropriately
                    if (property.PropertyType == typeof(Dictionary<string, JsonElement>))
                    {
                        IDictionary<string, JsonElement> dict = (IDictionary<string, JsonElement>)property.GetValue(deserializedObject);
                        if (dict == null)
                        {
                            dict = new Dictionary<string, JsonElement>();
                        }
                        dict.Add(propertyName, json);
                        property.SetValue(deserializedObject, dict);
                    }
                    else // has to be Dictionary<string, object>
                    {
                        IDictionary<string, object> dict = (IDictionary<string, object>)property.GetValue(deserializedObject);
                        if (dict == null)
                        {
                            dict = new Dictionary<string, object>();
                        }
                        dict.Add(propertyName, json);
                        property.SetValue(deserializedObject, dict);
                    }
                }
            }

            public override void Write(Utf8JsonWriter writer, TValue objToSerialize, JsonSerializerOptions options)
            {
                // start writing the serialization
                writer.WriteStartObject();

                // get all of the internal and public properties
                PropertyInfo[] properties = typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // start serializing
                SerializeProperties(properties, ref writer, objToSerialize, options);

                // wrap up the serialization
                writer.WriteEndObject();
            }

            private void SerializeProperties(PropertyInfo[] properties, ref Utf8JsonWriter writer, Object objToSerialize, JsonSerializerOptions options)
            {
                // loop through all of the object properties
                foreach (PropertyInfo property in properties)
                {
                    // check to see if we should ignore this property
                    if (IgnoreProperty(property, options)) continue;

                    // check to see if this property has the JsonExtensionDataAttribute
                    if (property.GetCustomAttribute<JsonExtensionDataAttribute>() != null)
                    {
                        // property has to be a dictionary to have the JsonExtensionDataAttribute
                        // get the dictionary
                        if (property.PropertyType == typeof(Dictionary<string, object>))
                        {
                            var val = (Dictionary<string, object>)property.GetValue(objToSerialize);

                            // work through the kvps
                            foreach (KeyValuePair<string, object> kvp in val)
                            {
                                // write the correct property name to the Json
                                writer.WritePropertyName(GetPropertyName(kvp.Key, options));

                                // write the value to the Json
                                JsonSerializer.Serialize(writer, kvp.Value, options);
                            }
                        }
                        else
                        {
                            var val = (Dictionary<string, JsonElement>)property.GetValue(objToSerialize);

                            // work through the kvps
                            foreach (KeyValuePair<string, JsonElement> kvp in val)
                            {
                                // write the correct property name to the Json
                                writer.WritePropertyName(GetPropertyName(kvp.Key, options));

                                // write the value to the Json
                                JsonSerializer.Serialize(writer, kvp.Value, options);
                            }
                        }

                        // move to next property
                        continue;
                    }

                    // get the value of the property
                    var value = property.GetValue(objToSerialize);

                    // look for nested objects and recursively call this function
                    // to ensure we follow these rules for nested object properties
                    if (property.PropertyType.IsNested)
                    {
                        // write the property name before writing the nested properties
                        writer.WritePropertyName(GetPropertyName(property, options));

                        // open this Json object
                        writer.WriteStartObject();

                        // get all of the internal and public properties
                        PropertyInfo[] props = (property.PropertyType).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        // make recursive call
                        SerializeProperties(props, ref writer, value, options);

                        // close this Json object
                        writer.WriteEndObject();

                        // move to next property
                        continue;
                    }

                    // if value is null and the property asks to be ignored when null via the JsonPropertyAttribute
                    if (value == null && HasIgnoreNullValueAttribute(property))
                    {
                        // do not include this value in the serialization
                        continue;
                    }

                    // if value is null, should we handle it anyway or skip it?
                    if (value == null && !HasIncludeNullAttribute(property) && options.IgnoreNullValues)
                    {
                        // do not include this value in the serialization
                        continue;
                    }

                    // write the correct property name to the Json
                    writer.WritePropertyName(GetPropertyName(property, options));

                    // write the value to the Json
                    JsonSerializer.Serialize(writer, value, options);
                }
            }
        }
    }
}
