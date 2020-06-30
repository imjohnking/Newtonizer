# Newtonizer
A [custom JSON converter](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to) for use with .NET Core [System.Text.Json serialization and deserialization](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to). Newtonizer provides access to "internal" class properties as well as adds granular control over null value handling. It intentionally works similar to Newtonsoft's JSON.NET library for these specific functions without having to use a third-party JSON library. Newtonizer supports all other existing built-in System.Text.Json serialization/deserialization functionality.

<br/>

## How to get started with Newtonizer

The easiest way to include Newtonizer in your Visual Studio project is to download the [NuGet package](https://www.nuget.org/packages/Take5ive.Json.Converters.Newtonizer/) directly in Visual Studio.  The package is named _Take5ive.Json.Converters.Newtonizer_.

Right-click your solution or project in "Solution Explorer" and select "Manage NuGet Packages":

![VS Menu](http://imjohnking.com/wp-content/uploads/2020/06/NuGet_1.png)

In the search box, type "Newtonizer":

![NuGet Search](http://imjohnking.com/wp-content/uploads/2020/06/Newtonizer_2.png)

Click the "Install" button:

![NuGet Install](http://imjohnking.com/wp-content/uploads/2020/06/Newtonizer_3.png)

Click "OK" and agree to any license screens that are presented. Installation takes just a few seconds and you're ready to go.

Alternatively you can download the source code directly from GitHub and include it in your project without NuGet; but why would you do it manually? :confused:

<br/>

## Registering Newtonizer in your project

Once you've included Newtonizer in your project, you have to [register the custom converter](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to#register-a-custom-converter) for use with serialization and deserialization. There are three ways you can do this:

* Add an instance of the converter class to the JsonSerializerOptions.Converters collection.

* Apply the [JsonConverter] attribute to the properties that require the custom converter.

* Apply the [JsonConverter] attribute to a class or a struct that represents a custom value type.

The first option is perhaps the simplest and easiest to understand method of registering Newtonizer. You can manually add an instance of the Newtonizer converter class to your JsonSerializerOptions.Converters collection and include these options when serializing or deserializing your objects:

```
var serializeOptions = new JsonSerializerOptions();
serializeOptions.Converters.Add(new Newtonizer(typeof(typeToRegister)));
jsonString = JsonSerializer.Serialize(objectToSerialize, serializeOptions);
```

**Note above:** You have to tell Newtonizer what type of classes it should convert (**_typeToRegister_**). If you specify a base class type, Newtonizer will be registered to convert that base class type and any derived classes of that base type.

The second and third options for registering Newtonizer as a custom converter both use the [JsonConverter] attribute; you either place it on a class property or directly on a class or struct:

```
[JsonConverter(typeof(Newtonizer(typeof(typeToRegister))))]
Your class, struct, or property declaration here
```
For more details on the second and third options, you can [read the Microsoft documentation here](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to#registration-sample---jsonconverter-on-a-property).

<br/>

## Using Newtonizer to Serialize/Deserialize

### How to work with "internal" properties

One of the main reasons I created this custom converter was for the ability to include internal class properties in the serialized object. Newtonsoft's JSON.NET library already has this functionality built-in.  However, for my project, I wanted to only use the .NET System.Text.Json library.  This required writing this custom converter.

Using Newtonizer on internal properties is actually very simple. Under normal circumstances, the JsonSerializer will ignore any internal properties completely; regardless of what attributes they may have been decorated with. However, if you decorate an internal property with the [**_JsonPropertyName_**] attribute, Newtonizer trusts that you added that attribute intentionally and it will be included in both serialization and deserialization while still using the System.Text.Json library. Keep in mind that [**_JsonPropertyName_**] is what instructs Newtonizer to include the internal property. Once you add that attribute, you can also add any other JsonAttribute from the System.Text.Json library and Newtonizer will acknowledge and comply with those attributes as well.

Let's look at an example class:

```
public class Person
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  
  [JsonPropertyName("ssn")]
  internal string SSN { get; set; }
}
```

Based on this code:

```
var person = new Person()
{
  FirstName = "John",
  LastName = "Doe",
  SSN = "123-45-6789"
};

var json = JsonSerializer.Serialize(person);
```

The string "json" will equal "{"FirstName":"John","LastName":"Doe"}"

However, if you register Newtonizer for the type of "Person" and include it in your JsonSerializerOptions as a converter:

```
var person = new Person()
{
  FirstName = "John",
  LastName = "Doe",
  SSN = "123-45-6789"
};

var jsonOptions = new JsonSerializerOptions();
jsonOptions.Converters.Add(new Newtonizer(typeof(Person)));

var json = JsonSerializer.Serialize(person, jsonOptions);
```

The resulting "json" string will equal "{"FirstName":"John","LastName":"Doe","ssn":"123-45-6789"}" 

_(notice the lowercase "ssn" as specified in the [JsonPropertyName] attribute)_

**Pretty simple right?  Just register Newtonizer as a converter and add some basic JsonAttributes and you're done!**

Also, keep in mind that you can combine the above with any other JsonSerializerOptions you want to use such as camelCase naming conventions or indented pretty print. You'll find the full list of available options on the [Microsoft documentation website here](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#customize-json-names-and-values).

### How to use the Take5ive.Json.Attributes.JsonPropertyAttribute

System.Text.Json in .NET Core 3.1 does not provide any granular control over how to handle "null" values in your JSON. It only provides a global way to specify to ignore null values (or not) using the [JsonSerializerOptions.IgnoreNullValues](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions.ignorenullvalues?view=netcore-3.1#System_Text_Json_JsonSerializerOptions_IgnoreNullValues) property. Newtonizer solves this problem by adding the _Take5ive.Json.Attributes.JsonPropertyAttribute_. This attribute is used exactly like the Newtonsoft JSON.NET library attribute without requiring that library.

#### _[JsonProperty(NullValueHandling = NullValueHandling.Include)]_

Sticking with our Person example, let's say you have the following class:

```
public class Person
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  
  [JsonPropertyName("ssn")]
  internal string SSN { get; set; }
  
  public string Address { get; set;}
  public string City { get; set; }
  public string State { get; set; }
  public string Zip { get; set; }
  
  [JsonProperty(NullValueHandling = NullValueHandling.Include)]
  public string Email { get; set; }
}
```

Notice how we've used the _[JsonProperty(NullValueHandling = NullValueHandling.Include)]_ attribute on the "Email" property. This is how we granularly tell Newtonizer that even if the global JsonSerializerOptions.IgnoreNullValues property has been set to "true", we want to include Email in the serialized JSON even if it is "null".

Here it is in action:

```
var person = new Person()
{
  FirstName = "John",
  SSN = "123-45-6789",
  City = "Lakeland",
  State = "FL"
};

var jsonOptions = new JsonSerializerOptions()
{
  jsonOptions.IgnoreNullValues = true;
};

jsonOptions.Converters.Add(new Newtonizer(typeof(Person)));

var json = JsonSerializer.Serialize(person, jsonOptions);
```

Once this code completes, the string "json" will have the following value:

"{"FirstName":"John","ssn":"123-45-6789","City":"Lakeland","State":"FL","Email":null}"

<br/>

#### _[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]_

Newtonizer also provides the _[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]_ attribute which works in exactly the opposite manner.  If we reuse the above example and simply change the _jsonOptions.IgnoreNullValues = true_ to _jsonOptions.IgnoreNullValues = false_ and change the _[JsonProperty]_ to _NullValueHandling.Ignore_ we get the example below:

```
public class Person
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  
  [JsonPropertyName("ssn")]
  internal string SSN { get; set; }
  
  public string Address { get; set;}
  public string City { get; set; }
  public string State { get; set; }
  public string Zip { get; set; }
  
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public string Email { get; set; }
}
```

```
var person = new Person()
{
  FirstName = "John",
  SSN = "123-45-6789",
  City = "Lakeland",
  State = "FL"
};

var jsonOptions = new JsonSerializerOptions()
{
  jsonOptions.IgnoreNullValues = false;
};

jsonOptions.Converters.Add(new Newtonizer(typeof(Person)));

var json = JsonSerializer.Serialize(person, jsonOptions);
```

Once this code completes, the string "json" will have the following value:

"{"FirstName":"John","LastName":null,"ssn":"123-45-6789","City":"Lakeland","State":"FL","Zip":null}"

Notice that all of the null values were included in the JSON string except "Email" because it was granularly controlled with the [JsonProperty] attribute.

## Final Thoughts

As we've seen, Newtonizer is easy to use and provides granular control over how you can serialize/deserialize objects. As a registered custom JSON converter in your .NET Core projects, Newtonizer just sits on top of the System.Text.Json library. As such, you can still use all of the other properties and options within the System.Text.Json library right alongside what Newtonizer provides. If you are not already familiar with how to use System.Text.Json, I encourage you to read this _[Microsoft article on "How to serialize and deserialze JSON in .NET](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialization-behavior)_.

If you have any questions, concerns, or find any issues, please feel free to reach out to me.
