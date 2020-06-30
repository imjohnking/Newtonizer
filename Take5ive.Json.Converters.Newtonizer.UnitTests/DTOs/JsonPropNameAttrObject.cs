using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class JsonPropNameAttrObject : BaseObject
    {
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        public int SomeNumber { get; set; }

        public bool SomeBooleanValue { get; set; }
    }
}
