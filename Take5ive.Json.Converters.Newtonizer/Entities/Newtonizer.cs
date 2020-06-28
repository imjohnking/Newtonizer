using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Take5ive.Json.Converters
{
    public partial class Newtonizer : JsonConverterFactory
    {
        private readonly Type _registeredForType;

        /// <summary>
        /// Newtonizer Custom JsonConverter
        /// </summary>
        /// <param name="RegisterForType">Register converter to handle this type or any subclass of this type.</param>
        public Newtonizer(Type RegisterForType)
        {
            _registeredForType = RegisterForType;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == _registeredForType || typeToConvert.IsSubclassOf(_registeredForType);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(NewtonizerInner<>).MakeGenericType(new Type[] { typeToConvert }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null);

            return converter;
        }
    }
}
