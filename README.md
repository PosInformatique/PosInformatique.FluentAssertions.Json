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
}
```

With the following instance:

```csharp
var customer = new Customer()
{
    Id = 1234,
    Name = "Gilles TOURREAU",
};
```

You would like to check this class is serializable into the following JSON object:

```json
{
    "id": 1234,
    "name": "Gilles TOURREAU",
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
    };

    var json = JsonSerializer.Serialize(customer);

    json.Should().Be("{\"id\":1234,\"name\":\"Gilles TOURREAU\"}");
    
    // Or
    json.Should().Be(@"{""id"":1234,""name"":""Gilles TOURREAU""}");
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
    };

    customer.Should().BeJsonSerializableInto(new
    {
        id = 1234,
        name = "Gilles TOURREAU",
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
    };

    json.Should().BeJsonDeserializableInto(new Customer()
    {
        Id = 1234,
        Name = "Gilles TOURREAU",
    });
}
```