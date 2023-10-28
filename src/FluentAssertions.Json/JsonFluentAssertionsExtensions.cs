﻿//-----------------------------------------------------------------------
// <copyright file="JsonFluentAssertionsExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace FluentAssertions
{
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FluentAssertions.Collections;
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
            BeJsonSerializableIntoCore<object>(assertions, expectedJson, GetSerializerOptions(options));
        }

        /// <summary>
        /// Check if the subject object in <paramref name="assertions"/> instance is serializable into the JSON object
        /// specified by the <paramref name="expectedJson"/>.
        /// </summary>
        /// <typeparam name="TBase">Base class type of the subject to use for the serialization. Defines explicitely the base type
        /// to assert the JSON serialization with base class using polymorphisme discriminator.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the object subject to check.</param>
        /// <param name="expectedJson">The object which represents the raw JSON object expected.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to use to assert the serialization. If not specified
        /// the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/> of the <see cref="FluentAssertionsJson.Configuration"/>
        /// will be used.</param>
        public static void BeJsonSerializableInto<TBase>(this ObjectAssertions assertions, object? expectedJson, JsonSerializerOptions? options = null)
        {
            BeJsonSerializableIntoCore<TBase>(assertions, expectedJson, GetSerializerOptions(options));
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

            BeJsonSerializableIntoCore<object>(assertions, expectedJson, optionsCopy);
        }

        /// <summary>
        /// Check if the subject object in <paramref name="assertions"/> instance is serializable into the JSON object
        /// specified by the <paramref name="expectedJson"/>.
        /// </summary>
        /// <typeparam name="TBase">Base class type of the subject to use for the serialization. Defines explicitely the base type
        /// to assert the JSON serialization with base class using polymorphisme discriminator.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the object subject to check.</param>
        /// <param name="expectedJson">The object which represents the raw JSON object expected.</param>
        /// <param name="configureOptions">Allows to change the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/>
        /// of the <see cref="FluentAssertionsJson.Configuration"/> used to assert the serialization.</param>
        public static void BeJsonSerializableInto<TBase>(this ObjectAssertions assertions, object? expectedJson, Action<JsonSerializerOptions> configureOptions)
        {
            var optionsCopy = new JsonSerializerOptions(FluentAssertionsJson.Configuration.JsonSerializerOptions);

            configureOptions(optionsCopy);

            BeJsonSerializableIntoCore<TBase>(assertions, expectedJson, optionsCopy);
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
            BeJsonDeserializableIntoCore(assertions.Subject, expectedObject, GetSerializerOptions(options));
        }

        /// <summary>
        /// Check if the JSON subject collection is deserializable into the specified <paramref name="expectedObject"/> argument.
        /// </summary>
        /// <typeparam name="TElement">Type of the element of the collection to check the JSON deserialization.</typeparam>
        /// <typeparam name="T">Type of the object to deserialize from JSON.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the JSON collection subject to deserialize.</param>
        /// <param name="expectedObject">Expected collection deserialized expected.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to use to assert the deserialization. If not specified
        /// the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/> of the <see cref="FluentAssertionsJson.Configuration"/>
        /// will be used.</param>
        public static void BeJsonDeserializableInto<TElement, T>(this GenericCollectionAssertions<TElement> assertions, T expectedObject, JsonSerializerOptions? options = null)
        {
            BeJsonDeserializableIntoCore(assertions.Subject, expectedObject, GetSerializerOptions(options));
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

            BeJsonDeserializableIntoCore(assertions.Subject, expectedObject, optionsCopy);
        }

        /// <summary>
        /// Check if the JSON subject collection is deserializable into the specified <paramref name="expectedObject"/> argument.
        /// </summary>
        /// <typeparam name="TElement">Type of the element of the collection to check the JSON deserialization.</typeparam>
        /// <typeparam name="T">Type of the object to deserialize from JSON.</typeparam>
        /// <param name="assertions"><see cref="ObjectAssertions"/> which contains the JSON collection subject to deserialize.</param>
        /// <param name="expectedObject">Expected collection deserialized expected.</param>
        /// <param name="configureOptions">Allows to change the default <see cref="IFluentAssertionsJsonConfiguration.JsonSerializerOptions"/>
        /// of the <see cref="FluentAssertionsJson.Configuration"/> used to assert the deserialization.</param>
        public static void BeJsonDeserializableInto<TElement, T>(this GenericCollectionAssertions<TElement> assertions, T expectedObject, Action<JsonSerializerOptions> configureOptions)
        {
            var optionsCopy = new JsonSerializerOptions(FluentAssertionsJson.Configuration.JsonSerializerOptions);

            configureOptions(optionsCopy);

            BeJsonDeserializableIntoCore(assertions.Subject, expectedObject, optionsCopy);
        }

        private static void BeJsonSerializableIntoCore<TBase>(ObjectAssertions assertions, object? expectedJson, JsonSerializerOptions options)
        {
            if (assertions.Subject is not null && assertions.Subject is not TBase)
            {
                throw new ArgumentException($"The '{typeof(TBase).FullName}' class is not a base class of the '{assertions.Subject.GetType().FullName}' type.", nameof(TBase));
            }

            var jsonString = JsonSerializer.Serialize((TBase)assertions.Subject!, options);

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

            var errors = JsonElementComparer.Compare(deserializedJsonDocument!, expectedJson);

            if (errors.Any())
            {
                Services.ThrowException(errors.First());
            }
        }

        private static void BeJsonDeserializableIntoCore<T>(object subject, T expectedObject, JsonSerializerOptions options)
        {
            var jsonText = JsonSerializer.Serialize(subject, options);
            var deserializedObject = JsonSerializer.Deserialize<T>(jsonText, options);

            deserializedObject.Should().BeEquivalentTo(expectedObject, opt =>
            {
                opt.Using<object>(ctx =>
                {
                    if (ctx.Subject is JsonElement element)
                    {
                        var path = ReflectionHelper.GetJsonPath(deserializedObject!.GetType(), ctx.SelectedNode.PathAndName);

                        var errors = JsonElementComparer.Compare(element, ctx.Expectation, path);

                        if (errors.Any())
                        {
                            Services.ThrowException(errors.First());
                        }
                    }
                })
                .When(member => member.Type == typeof(object));

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
    }
}
