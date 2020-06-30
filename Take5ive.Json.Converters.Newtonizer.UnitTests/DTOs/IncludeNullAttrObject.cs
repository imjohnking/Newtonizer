using Take5ive.Json.Attributes;

namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class IncludeNullAttrObject : BaseObject
    {
        public string FirstName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string LastName { get; set; }

        public int SomeNumber { get; set; }

        public bool SomeBooleanValue { get; set; }
    }
}
