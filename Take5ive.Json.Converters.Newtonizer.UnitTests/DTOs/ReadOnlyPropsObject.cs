namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class ReadOnlyPropsObject : BaseObject
    {
        public string FirstName { get; } = "John";

        public string LastName { get; set; }

        public int SomeNumber { get; set; }

        public bool SomeBooleanValue { get; set; }
    }
}
