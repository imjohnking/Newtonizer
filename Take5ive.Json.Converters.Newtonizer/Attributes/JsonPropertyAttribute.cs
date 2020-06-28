using System;

namespace Take5ive.Json.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class JsonPropertyAttribute : System.Attribute
    {
        public NullValueHandling NullValueHandling;
    }

    public enum NullValueHandling
    {
        Ignore,
        Include
    }
}
