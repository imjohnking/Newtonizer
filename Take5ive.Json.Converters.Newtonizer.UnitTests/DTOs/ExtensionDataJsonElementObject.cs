using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class ExtensionDataJsonElementObject : BaseObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int SomeNumber { get; set; }

        public bool SomeBooleanValue { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtraData { get; set; }
    }
}
