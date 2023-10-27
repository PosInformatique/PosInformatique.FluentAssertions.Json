# PosInformatique.FluentAssertions.Json
PosInformatique.FluentAssertions.Json is a library to assert JSON serialization using the *Fluent Assertions* library style coding.

## Installing from NuGet
The [PosInformatique.FluentAssertions.Json](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/)
library is available directly on the
[![Nuget](https://img.shields.io/nuget/v/PosInformatique.FluentAssertions.Json)](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/)
official website.

To download and install the library to your Visual Studio unit test projects use the following NuGet command line 

```
Install-Package PosInformatique.FluentAssertions.Json
```

## How it is work?
Imagine that you have the following JSON class:

```csharp
public class Customer
{
    [JsonPropertyName("id")]
    [JsonPropertyOrder(1)]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [JsonPropertyOrder(2)]
    public string Name { get; set; }

    [JsonPropertyName("gender")]
    [JsonPropertyOrder(3)]
    public Gender Gender { get; set; }
}

public enum Gender
{
    None = 0,
    Male = 100,
    Female = 200,
}
```

With the following instance:

```csharp
var customer = new Customer()
{
    Id = 1234,
    Name = "Gilles TOURREAU",
    Gender = Gender.Male,
};
```

You would like to check this class is serializable into the following JSON object:

```json
{
    "id": 1234,
    "name": "Gilles TOURREAU",
    "gender": 100,
}
```

Using standard assertions you should write the following code :fearful::
```
[Fact]
public void Serialization()
{
    var customer = new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    };

    var json = JsonSerializer.Serialize(customer);

    json.Should().Be("{\"id\":1234,\"name\":\"Gilles TOURREAU\",\"gender\":100}");
    
    // Or
    json.Should().Be(@"{""id"":1234,""name"":""Gilles TOURREAU"",""gender"":100}");
}
```

With the following kind of exception when the unit test is incorrect:
![Ugly exception](https://raw.githubusercontent.com/PosInformatique/PosInformatique.FluentAssertions.Json/main/docs/UglyExceptionExample.png)

As you can see the previous code is not sexy to read (and to write!) and the exception is
hard to understand...

## Test the serialization of a .NET Object to a JSON object.
With the new fluent style using this library you can write previous unit test like that:

```csharp
[Fact]
public void Serialization()
{
    var customer = new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    };

    customer.Should().BeJsonSerializableInto(new
    {
        id = 1234,
        name = "Gilles TOURREAU",
        gender = 100,
    });
}
```

And when an exception is occured, the exception message contains the JSON path of the property which is error:
![Pretty exception](https://raw.githubusercontent.com/PosInformatique/PosInformatique.FluentAssertions.Json/main/docs/PrettyExceptionSample.png)

### Test the deserialization of a JSON object to a .NET Object
You can in the same way test the deserialization JSON object into a .NET object.

```
[Fact]
public void Deserialization()
{
    var json = new
    {
        id = 1234,
        name = "Gilles TOURREAU",
        gender = 100,
    };

    json.Should().BeJsonDeserializableInto(new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    });
}
```

## Test polymorphisme serialization with property discriminator
With the .NET 7.0 version of the `System.Text.Json` it is possible to serialize and deserialize JSON object
with property discriminators for polymorphism scenario
(See the [How to serialize properties of derived classes with System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-7-0))
topic for more information.
> It is possible also to use the polymorphism JSON serialization with previous version of .NET using a custom `JsonConverter`.
See the [How to serialize properties of derived classes with System.Text.Json (.NET 6.0)](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-6-0)
for more information.

Imagine you have the following classes hierarchy:

```csharp
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ThreeDimensionalPoint), typeDiscriminator: "3d")]
private class BasePoint
{
    [JsonPropertyOrder(1)]
    public int X { get; set; }

    [JsonPropertyOrder(2)]
    public int Y { get; set; }
}

private class ThreeDimensionalPoint : BasePoint
{
    [JsonPropertyOrder(3)]
    public int Z { get; set; }
}
```

And you would like to assert that the serialization of `ThreeDimensionalPoint` instance generate the following JSON content
when calling the `JsonSerializer.Serialize<T>()` method with `BasePoint` as generic argument:

```csharp
var point = new ThreeDimensionalPoint()
{
    X = 1,
    Y = 2,
    Z = 3,
}

var json = JsonSerializer.Serialize<BasePoint>();
```

```json
{
    "type": "3d",
    "X": 1,
    "Y": 2,
    "Z": 3, 
}
```

This is the assertion to write using the [PosInformatique.FluentAssertions.Json library](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/):


```csharp
point.Should().BeJsonSerializableInto<BasePoint>(new
{
    myType = "3d",
    X = 1,
    Y = 2,
    Z = 3,
});
```

> **NOTE:** If you don't specify the `BasePoint` generic argument, the library will use the default behavior of the `JsonSerializer.Serialize()` 
(with no generic argument), which will generate (and assert!) the following JSON object:
>```json
>{
>    "X": 1,
>    "Y": 2,
>    "Z": 3, 
>}
>```
> As you can see there is no discriminator property generate because the .NET `JsonSerializer.Serialize()` will use the type of the instance
> instead of an explicit type of derived class.

## Change the JsonSerializer options

### Change globally the JsonSerializer options
The JSON serialization and deserialization assertions use the default instance of the
`JsonSerializerOptions`.

It is possible to change globally this default options by accessing to the static instance
of `FluentAssertionsJson.Configuration.JsonSerializerOptions`.

For example, if you use xUnit test engine and you want to apply the `JsonStringEnumConverter`
for all the unit tests in the `PosInformatique.JsonModels.Tests`, create the following
xUnit extensions class and apply the assembly `XunitTestFrameworkAttribute` to reference this class:

```csharp
[assembly: TestFramework("PosInformatique.JsonModels.Tests.JsonModelsTestFramework", "PosInformatique.JsonModels.Tests")]

namespace PosInformatique.JsonModels.Tests
{
    using System.Text.Json.Serialization;
    using PosInformatique.FluentAssertions.Json;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class JsonModelsTestFramework : XunitTestFramework
    {
        public FunctionsTestFramework(IMessageSink messageSink)
            : base(messageSink)
        {
            FluentAssertionsJson.Configuration.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }
    }
}
```

### Defines the JsonSerializer options specifically on an unit test.
It is possible when calling the `BeJsonSerializableInto()` and `BeJsonDeserializableInto()` methods
to specify the `JsonSerializerOptions` used for the serialization and deserialization process
during the assertion.

For example, to use the `JsonStringEnumConverter` to serialize the enum into a string value:

```csharp
[Fact]
public void Serialization()
{
    var customer = new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    };

    var options = new JsonSerializerOptions()
    {
        new JsonStringEnumConverter(),
    };

    customer.Should().BeJsonSerializableInto(new
    {
        id = 1234,
        name = "Gilles TOURREAU",
        gender = "Male",
    },
    options);
}

[Fact]
public void Deserialization()
{
    var json = new
    {
        id = 1234,
        name = "Gilles TOURREAU",
        gender = "Male",
    };

    var options = new JsonSerializerOptions()
    {
        new JsonStringEnumConverter(),
    };

    json.Should().BeJsonDeserializableInto(new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    },
    options);
}
```

### Changes the global JsonSerializerOptions for a specific assertion.
It is possible to changes the global `FluentAssertionsJson.Configuration.JsonSerializerOptions`
for a specific assertion using a configuration lambda expression.

For example, if you have defines globally the `JsonStringEnumConverter` converter for all the unit tests
(see the previous examples), but you would like to remove the existing converter in the specific unit tests:

```csharp
[Fact]
public void Serialization()
{
    var customer = new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    };

    customer.Should().BeJsonSerializableInto(new
    {
        id = 1234,
        name = "Gilles TOURREAU",
        gender = "Male",
    },
    opt =>
    {
        opt.Converters.Clear();
    });
}

[Fact]
public void Deserialization()
{
    var json = new
    {
        id = 1234,
        name = "Gilles TOURREAU",
        gender = "Male",
    };

    var options = new JsonSerializerOptions()
    {
        new JsonStringEnumConverter(),
    };

    json.Should().BeJsonDeserializableInto(new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
        Gender = Gender.Male,
    },
    opt =>
    {
        opt.Converters.Clear();
    });
}
```

> **NOTE**: The lambda expression allows to update the global
`FluentAssertionsJson.Configuration.JsonSerializerOptions` instance during the call
of the `BeJsonSerializableInto()` or `BeJsonDeserializableInto()` methods. Global previous
`JsonSerializerOptions` will be restored at the end of the assertion.

## Library dependencies
- The [PosInformatique.FluentAssertions.Json](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/) library
target the .NET Standard 2.0 and can be used with various of .NET architecture (.NET Core, .NET Framework,...).

- The [PosInformatique.FluentAssertions.Json](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/) library
use the 5.0.0 version of the [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) NuGet package
and can be used with old projects that target this library version and earlier.