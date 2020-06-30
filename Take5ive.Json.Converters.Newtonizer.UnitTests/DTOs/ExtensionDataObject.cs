using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class ExtensionDataObject : BaseObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int SomeNumber { get; set; }

        public bool SomeBooleanValue { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> ExtraData { get; set; }
    }
}
