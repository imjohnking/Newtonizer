using System.Text.Json;
using Take5ive.Json.Converters.UnitTests.DTOs;
using Xunit;
using static Take5ive.Json.Converters.UnitTests.Builders.BuildDTOs;

namespace Take5ive.Json.Converters.UnitTests
{
    public class Newtonizer_Serialization_Should
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public Newtonizer_Serialization_Should()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new Newtonizer(typeof(BaseObject)));
        }

        [Fact]
        public void SerializeSimpleObject()
        {
            var obj = GetSimpleObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void DuringSerialization_RespectNamingPolicy()
        {
            _jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var obj = GetSimpleObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = "{\"firstName\":\"John\",\"lastName\":\"Doe\",\"someNumber\":100,\"someBooleanValue\":true}";

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void DuringSerialization_IgnoreNullValues()
        {
            _jsonSerializerOptions.IgnoreNullValues = true;

            var obj = GetSimpleObject();

            obj.LastName = null;

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = "{\"FirstName\":\"John\",\"SomeNumber\":100,\"SomeBooleanValue\":true}";

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void SerializeWithNestedObject()
        {
            var obj = GetHasNestedObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void SerializeWithPropNameAttr()
        {
            var obj = GetJsonPropNameAttrObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void SerializeWithInternalProps()
        {
            var obj = GetInternalPropertiesObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void DuringSerialization_RespectIgnoreAttr()
        {
            var obj = GetJsonIgnoreAttrObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void DuringSerialization_IgnoreNullValuesWithAttr()
        {
            _jsonSerializerOptions.IgnoreNullValues = false;

            var obj = GetIgnoreNullAttrObject();

            var serializedObject = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObject);
        }

        [Fact]
        public void DuringSerialization_IncludeNullValuesWithAttr()
        {
            _jsonSerializerOptions.IgnoreNullValues = true;

            var obj = GetIncludeNullAttrObject();

            var serializedObject = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObject);
        }

        [Fact]
        public void DuringSerialization_ExcludeReadOnlyProps()
        {
            _jsonSerializerOptions.IgnoreReadOnlyProperties = true;

            var obj = GetReadOnlyPropsObject();

            var serializedObject = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObject);
        }

        [Fact]
        public void SerializeExtensionData_WithObjectDictionary()
        {
            var obj = GetExtensionDataObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }

        [Fact]
        public void SerializeExtensionData_WithJsonElementDictionary()
        {
            var obj = GetExtensionDataJsonElementObject();

            var serializedObj = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

            var expectedResult = obj.Json;

            Assert.Equal(expectedResult, serializedObj);
        }
    }
}
