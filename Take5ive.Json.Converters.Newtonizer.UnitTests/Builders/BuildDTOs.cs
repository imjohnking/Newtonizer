using System.Collections.Generic;
using System.Text.Json;
using Take5ive.Json.Converters.UnitTests.DTOs;

namespace Take5ive.Json.Converters.UnitTests.Builders
{
    public static class BuildDTOs
    {
        public static SimpleObject GetSimpleObject()
        {
            var obj = new SimpleObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static HasNestedObject GetHasNestedObject()
        {
            HasNestedObject obj = new HasNestedObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"Nested\":{\"SomeStrings\":[\"One\",\"Two\",\"Three\",\"Four\",\"Five\"],\"SomeNumbers\":[1,2,3,4,5]},\"FirstName\":\"John\",\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            obj.Nested.SomeNumbers = new int[] { 1, 2, 3, 4, 5 };
            obj.Nested.SomeStrings = new string[] { "One", "Two", "Three", "Four", "Five" };

            return obj;
        }
        public static JsonPropNameAttrObject GetJsonPropNameAttrObject()
        {
            var obj = new JsonPropNameAttrObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"last_name\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static InternalPropertiesObject GetInternalPropertiesObject()
        {
            var obj = new InternalPropertiesObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                InternalString = "Internal String Stuff",
                AnotherInternalString = "More internal string stuff",
                Json = "{\"internal_string\":\"Internal String Stuff\",\"FirstName\":\"John\",\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static JsonIgnoreAttrObject GetJsonIgnoreAttrObject()
        {
            var obj = new JsonIgnoreAttrObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"LastName\":\"Doe\",\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static IgnoreNullAttrObject GetIgnoreNullAttrObject()
        {
            var obj = new IgnoreNullAttrObject()
            {
                FirstName = "John",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static IncludeNullAttrObject GetIncludeNullAttrObject()
        {
            var obj = new IncludeNullAttrObject()
            {
                FirstName = "John",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"LastName\":null,\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static ReadOnlyPropsObject GetReadOnlyPropsObject()
        {
            var obj = new ReadOnlyPropsObject()
            {
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true}"
            };

            return obj;
        }

        public static ExtensionDataObject GetExtensionDataObject()
        {
            var obj = new ExtensionDataObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true,\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}"
            };

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("key1", "value1");
            dict.Add("key2", "value2");
            dict.Add("key3", "value3");

            obj.ExtraData = dict;

            return obj;
        }

        public static ExtensionDataJsonElementObject GetExtensionDataJsonElementObject()
        {
            var obj = new ExtensionDataJsonElementObject()
            {
                FirstName = "John",
                LastName = "Doe",
                SomeBooleanValue = true,
                SomeNumber = 100,
                Json = "{\"FirstName\":\"John\",\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true,\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}"
            };

            var jdoc = JsonDocument.Parse("{\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}");

            Dictionary<string, JsonElement> dict = new Dictionary<string, JsonElement>();

            foreach (JsonProperty e in jdoc.RootElement.EnumerateObject())
            {
                dict.Add(e.Name, e.Value);
            }

            obj.ExtraData = dict;

            return obj;
        }
    }
}
