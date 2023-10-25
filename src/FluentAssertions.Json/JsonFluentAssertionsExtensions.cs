//-----------------------------------------------------------------------
// <copyright file="JsonFluentAssertionsExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace FluentAssertions
{
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FluentAssertions.Common;
    using FluentAssertions.Equivalency;
    using FluentAssertions.Primitives;
    using PosInformatique.FluentAssertions.Json;

    /// <summary>
    /// Contains extension methods to check if an object is serializable or deserializable to JSON object.
    /// </summary>
    public static class JsonFluentAssertionsExtensions
    {
        /// <summary>
        /// Check if the subject object in <paramref name="assertions"/> instance is serializable into the JSON object
        /// specified by the <paramref name="expectedJson"/>.
        /// </summary>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the object subject to check.</param>
        /// <param name="expectedJson">The object which represents the raw JSON object expected.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to use to assert the serialization. If not specified
        /// the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/> of the <see cref="FluentAssertionsJson.Configuration"/>
        /// will be used.</param>
        public static void BeJsonSerializableInto(this ObjectAssertions assertions, object? expectedJson, JsonSerializerOptions? options = null)
        {
            BeJsonSerializableIntoCore(assertions, expectedJson, GetSerializerOptions(options));
        }

        /// <summary>
        /// Check if the subject object in <paramref name="assertions"/> instance is serializable into the JSON object
        /// specified by the <paramref name="expectedJson"/>.
        /// </summary>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the object subject to check.</param>
        /// <param name="expectedJson">The object which represents the raw JSON object expected.</param>
        /// <param name="configureOptions">Allows to change the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/>
        /// of the <see cref="FluentAssertionsJson.Configuration"/> used to assert the serialization.</param>
        public static void BeJsonSerializableInto(this ObjectAssertions assertions, object? expectedJson, Action<JsonSerializerOptions> configureOptions)
        {
            var optionsCopy = new JsonSerializerOptions(FluentAssertionsJson.Configuration.JsonSerializerOptions);

            configureOptions(optionsCopy);

            BeJsonSerializableIntoCore(assertions, expectedJson, optionsCopy);
        }

        /// <summary>
        /// Check if the JSON subject object is deserializable into the specified <paramref name="expectedObject"/> argument.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize from JSON.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the JSON object subject to deserialize.</param>
        /// <param name="expectedObject">Expected object deserialized expected.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to use to assert the deserialization. If not specified
        /// the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/> of the <see cref="FluentAssertionsJson.Configuration"/>
        /// will be used.</param>
        public static void BeJsonDeserializableInto<T>(this ObjectAssertions assertions, T expectedObject, JsonSerializerOptions? options = null)
        {
            BeJsonDeserializableIntoCore(assertions, expectedObject, GetSerializerOptions(options));
        }

        /// <summary>
        /// Check if the JSON subject object is deserializable into the specified <paramref name="expectedObject"/> argument.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize from JSON.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the JSON object subject to deserialize.</param>
        /// <param name="expectedObject">Expected object deserialized expected.</param>
        /// <param name="configureOptions">Allows to change the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/>
        /// of the <see cref="FluentAssertionsJson.Configuration"/> used to assert the deserialization.</param>
        public static void BeJsonDeserializableInto<T>(this ObjectAssertions assertions, T expectedObject, Action<JsonSerializerOptions> configureOptions)
        {
            var optionsCopy = new JsonSerializerOptions(FluentAssertionsJson.Configuration.JsonSerializerOptions);

            configureOptions(optionsCopy);

            BeJsonDeserializableIntoCore(assertions, expectedObject, optionsCopy);
        }

        private static void BeJsonSerializableIntoCore(this ObjectAssertions assertions, object? expectedJson, JsonSerializerOptions options)
        {
            var jsonString = JsonSerializer.Serialize(assertions.Subject, options);

            var deserializedJsonDocument = JsonSerializer.Deserialize<JsonDocument>(jsonString, options);

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

            var expectedJsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(expectedJson, options));

            Compare(deserializedJsonDocument!, expectedJsonDocument);
        }

        private static void BeJsonDeserializableIntoCore<T>(this ObjectAssertions assertions, T expectedObject, JsonSerializerOptions options)
        {
            var jsonText = JsonSerializer.Serialize(assertions.Subject, options);
            var deserializedObject = JsonSerializer.Deserialize<T>(jsonText, options);

            deserializedObject.Should().BeEquivalentTo(expectedObject, opt =>
            {
                return opt.Excluding(member => IsIgnoredProperty(member));
            });
        }

        private static JsonSerializerOptions GetSerializerOptions(JsonSerializerOptions? options)
        {
            if (options is null)
            {
                return FluentAssertionsJson.Configuration.JsonSerializerOptions;
            }

            return options;
        }

        private static bool IsIgnoredProperty(IMemberInfo member)
        {
            var property = member.DeclaringType.GetProperty(member.Name);

            var attribute = property.GetCustomAttribute<JsonIgnoreAttribute>();

            if (attribute is not null)
            {
                if (attribute.Condition == JsonIgnoreCondition.Always)
                {
                    return true;
                }
            }

            return false;
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
