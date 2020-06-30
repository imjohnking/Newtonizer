namespace Take5ive.Json.Converters.UnitTests.DTOs
{
    public class HasNestedObject : SimpleObject
    {
        public HasNestedObject()
        {
            Nested = new NestedObject();
        }

        public NestedObject Nested { get; set; }

        public class NestedObject
        {
            public string[] SomeStrings { get; set; }

            public int[] SomeNumbers { get; set; }
        }
    }
}
