//-----------------------------------------------------------------------
// <copyright file="JsonFluentAssertionsExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace FluentAssertions
{
    using System.Diagnostics;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FluentAssertions.Common;
    using FluentAssertions.Primitives;

    /// <summary>
    /// Contains extension methods to check if an object is serializable or deserializable to JSON object.
    /// </summary>
    [DebuggerNonUserCode]
    public static class JsonFluentAssertionsExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializationOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new JsonStringEnumConverter(),
            },
        };

        /// <summary>
        /// Check if the subject object in <paramref name="assertions"/> instance is serializable into the JSON object
        /// specified by the <paramref name="expectedJson"/>.
        /// </summary>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the object subject to check.</param>
        /// <param name="expectedJson">The object which represents the raw JSON object expected.</param>
        public static void BeJsonSerializableInto(this ObjectAssertions assertions, object? expectedJson)
        {
            var jsonString = JsonSerializer.Serialize(assertions.Subject, JsonSerializationOptions);

            var deserializedJsonDocument = JsonSerializer.Deserialize<JsonDocument>(jsonString, JsonSerializationOptions);

            if (deserializedJsonDocument is null)
            {
                if (expectedJson != null)
                {
                    Services.ThrowException("A JSON object was expected.");
                }
                else
                {
                    return;
                }
            }

            var expectedJsonDocument = JsonSerializer.SerializeToDocument(expectedJson, JsonSerializationOptions);

            Compare(deserializedJsonDocument!, expectedJsonDocument);
        }

        /// <summary>
        /// Check if the JSON subject object is deserializable into the specified <paramref name="expectedObject"/> argument.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize from JSON.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the JSON object subject to deserialize.</param>
        /// <param name="expectedObject">Expected object deserialized expected.</param>
        public static void BeJsonDeserializableInto<T>(this ObjectAssertions assertions, T expectedObject)
        {
            var jsonText = JsonSerializer.Serialize(assertions.Subject, JsonSerializationOptions);
            var deserializedObject = JsonSerializer.Deserialize<T>(jsonText, JsonSerializationOptions);

            deserializedObject.Should().BeEquivalentTo(expectedObject);
        }

        private static void Compare(JsonDocument document, JsonDocument expected)
        {
            var path = new Stack<string>();

            path.Push("$");

            Compare(document.RootElement, expected.RootElement, path);

            path.Pop();
        }

        private static void Compare(JsonElement element, JsonElement expected, Stack<string> path)
        {
            if (element.ValueKind != expected.ValueKind)
            {
                Services.ThrowException($"{GetPath(path)}: Expected property to be '{expected.ValueKind}' type instead of '{element.ValueKind}' type.");
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                var value = element.GetString();
                var expectedValue = expected.GetString();

                if (value != expectedValue)
                {
                    Services.ThrowException($"{GetPath(path)}: Expected '{expectedValue}' instead of '{value}'.");
                }
            }
            else if (element.ValueKind == JsonValueKind.Number)
            {
                var value = element.GetDouble();
                var expectedValue = expected.GetDouble();

                if (value != expectedValue)
                {
                    Services.ThrowException($"{GetPath(path)}: Expected '{expectedValue}' instead of '{value}'.");
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                var expectedPropertyEnumerator = expected.EnumerateObject();

                foreach (var property in element.EnumerateObject())
                {
                    if (!expectedPropertyEnumerator.MoveNext())
                    {
                        Services.ThrowException($"{GetPath(path)}: Expected no property but found '{property.Name}' property.");
                    }

                    var expectedProperty = expectedPropertyEnumerator.Current;

                    if (property.Name != expectedProperty.Name)
                    {
                        Services.ThrowException($"{GetPath(path)}: Expected property with the '{expectedProperty.Name}' name but found '{property.Name}' instead.");
                    }

                    path.Push("." + property.Name);

                    Compare(property.Value, expectedProperty.Value, path);

                    path.Pop();
                }

                if (expectedPropertyEnumerator.MoveNext())
                {
                    Services.ThrowException($"{GetPath(path)}: Expected '{expectedPropertyEnumerator.Current.Name}' property but found no property.");
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                var expectedArrayEnumerator = expected.EnumerateArray();
                var index = 0;

                foreach (var item in element.EnumerateArray())
                {
                    if (!expectedArrayEnumerator.MoveNext())
                    {
                        var actualCount = element.EnumerateArray().Count();

                        Services.ThrowException($"{GetPath(path)}: Expected {index} item(s) but found {actualCount}.");
                    }

                    var expectedItem = expectedArrayEnumerator.Current;

                    path.Push($"[{index}]");

                    Compare(item, expectedItem, path);

                    path.Pop();

                    index++;
                }

                if (expectedArrayEnumerator.MoveNext())
                {
                    var expectedCount = expected.EnumerateArray().Count();

                    Services.ThrowException($"{GetPath(path)}: Expected {expectedCount} item(s) but found {index}.");
                }
            }
            else if (element.ValueKind == JsonValueKind.True)
            {
                return;
            }
            else if (element.ValueKind == JsonValueKind.False)
            {
                return;
            }
            else
            {
                return;
            }
        }

        private static string GetPath(IEnumerable<string> path)
        {
            return string.Concat(path.Reverse());
        }
    }
}
