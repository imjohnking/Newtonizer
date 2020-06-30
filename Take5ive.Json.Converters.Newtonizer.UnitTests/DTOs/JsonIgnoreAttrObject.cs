using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class JsonIgnoreAttrObject : BaseObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonIgnore]
        public int SomeNumber { get; set; }

        public bool SomeBooleanValue { get; set; }
    }
}
