using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class InternalPropertiesObject : SimpleObject
    {
        [JsonPropertyName("internal_string")]
        internal string InternalString { get; set; }

        internal string AnotherInternalString { get; set; }
    }
}
