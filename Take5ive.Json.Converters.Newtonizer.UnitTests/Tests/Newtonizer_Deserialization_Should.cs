using System.Text.Json;
using Take5ive.Json.Converters.UnitTests.DTOs;
using Take5ive.Json.Converters.UnitTests.Entities;
using Xunit;
using static Take5ive.Json.Converters.UnitTests.Builders.BuildDTOs;

namespace Take5ive.Json.Converters.UnitTests
{
    public class Newtonizer_Deserialization_Should
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public Newtonizer_Deserialization_Should()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new Newtonizer(typeof(BaseObject)));
        }

        [Fact]
        public void DeserializeSimpleObject()
        {
            var obj = GetSimpleObject();

            var json = obj.Json;

            var deserailizedObj = JsonSerializer.Deserialize<SimpleObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserailizedObj, new CustomComparer<SimpleObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DuringDeserialization_RespectNamingPolicy()
        {
            _jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var obj = GetSimpleObject();

            var json = "{\"firstName\":\"John\",\"lastName\":\"Doe\",\"someNumber\":100,\"someBooleanValue\":true}";

            var deserializedObj = JsonSerializer.Deserialize<SimpleObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<SimpleObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DuringDeserialization_IgnoreNullValues()
        {
            _jsonSerializerOptions.IgnoreNullValues = true;

            var obj = GetSimpleObject();

            obj.LastName = null;

            var json = "{\"FirstName\":\"John\",\"SomeNumber\":100,\"SomeBooleanValue\":true}";

            var deserializedObj = JsonSerializer.Deserialize<SimpleObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<SimpleObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DeserializeWithNestedObject()
        {
            var obj = GetHasNestedObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<HasNestedObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<HasNestedObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DeserializeWithPropNameAttr()
        {
            var obj = GetJsonPropNameAttrObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<JsonPropNameAttrObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<JsonPropNameAttrObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DeserializeWithInternalProps()
        {
            var obj = GetInternalPropertiesObject();

            var json = obj.Json;

            var deserailizedObj = JsonSerializer.Deserialize<InternalPropertiesObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserailizedObj, new CustomComparer<InternalPropertiesObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DuringDeerialization_RespectIgnoreAttr()
        {
            var obj = GetJsonIgnoreAttrObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<JsonIgnoreAttrObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<JsonIgnoreAttrObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DuringDeserialization_IgnoreNullValuesWithAttr()
        {
            _jsonSerializerOptions.IgnoreNullValues = false;

            var obj = GetIgnoreNullAttrObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<IgnoreNullAttrObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<IgnoreNullAttrObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DuringDeserialization_IncludeNullValuesWithAttr()
        {
            _jsonSerializerOptions.IgnoreNullValues = true;

            var obj = GetIncludeNullAttrObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<IncludeNullAttrObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<IncludeNullAttrObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DuringDeserialization_ExcludeReadOnlyProps()
        {
            _jsonSerializerOptions.IgnoreReadOnlyProperties = true;

            var obj = GetReadOnlyPropsObject();

            var json = "{\"FirstName\":\"Dave\",\"LastName\":\"Doe\",\"SomeNumber\":100,\"SomeBooleanValue\":true}";

            var deserializedObj = JsonSerializer.Deserialize<ReadOnlyPropsObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<ReadOnlyPropsObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DeserializeExtensionData_WithObjectDictionary()
        {
            var obj = GetExtensionDataObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<ExtensionDataObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<ExtensionDataObject>(_jsonSerializerOptions));
        }

        [Fact]
        public void DeserializeExtensionData_WithJsonElementDictionary()
        {
            var obj = GetExtensionDataJsonElementObject();

            var json = obj.Json;

            var deserializedObj = JsonSerializer.Deserialize<ExtensionDataJsonElementObject>(json, _jsonSerializerOptions);

            Assert.Equal(obj, deserializedObj, new CustomComparer<ExtensionDataJsonElementObject>(_jsonSerializerOptions));
        }
    }
}
