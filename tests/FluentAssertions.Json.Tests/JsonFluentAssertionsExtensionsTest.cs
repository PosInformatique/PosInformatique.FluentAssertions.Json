//-----------------------------------------------------------------------
// <copyright file="JsonFluentAssertionsExtensionsTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace FluentAssertions.Json.Tests
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Xunit.Sdk;

    public class JsonFluentAssertionsExtensionsTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BeJsonSerializableInto(bool booleanValue)
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "The string value",
                Int32Property = 1234,
                BooleanProperty = booleanValue,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject()
                {
                    InnerStringProperty = "Inner string value",
                },
                CollectionInt32 = new List<int>
                {
                    10,
                    20,
                },
                CollectionObjects = new List<JsonSerializableClassInnerObject>()
                {
                    new JsonSerializableClassInnerObject()
                    {
                        InnerStringProperty = "Inner object 1",
                    },
                    new JsonSerializableClassInnerObject()
                    {
                        InnerStringProperty = "Inner object 2",
                    },
                },
            };

            json.Should().BeJsonSerializableInto(new
            {
                string_property = "The string value",
                int32_property = 1234,
                boolean_property = booleanValue,
                null_property = (string)null,
                inner_object = new
                {
                    inner_string_property = "Inner string value",
                },
                collection_int = new[]
                {
                    10,
                    20,
                },
                collection_object = new[]
                {
                    new
                    {
                        inner_string_property = "Inner object 1",
                    },
                    new
                    {
                        inner_string_property = "Inner object 2",
                    },
                },
            });
        }

        [Fact]
        public void BeJsonSerializableInto_StringPropertyValueDifferent()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "Actual value",
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "Expected value",
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.string_property: Expected 'Expected value' instead of 'Actual value'.");
        }

        [Fact]
        public void BeJsonSerializableInto_Int32PropertyValueDifferent()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "Actual value",
                Int32Property = 1234,
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "Actual value",
                    int32_property = 100,
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.int32_property: Expected '100' instead of '1234'.");
        }

        [Theory]
        [InlineData(true, "True", "False")]
        [InlineData(false, "False", "True")]
        public void BeJsonSerializableInto_BooleanPropertyValueDifferent(bool value, string actualValueString, string expectedValueString)
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "Actual value",
                Int32Property = 1234,
                BooleanProperty = value,
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "Actual value",
                    int32_property = 1234,
                    boolean_property = !value,
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.boolean_property: Expected property to be '{expectedValueString}' type instead of '{actualValueString}' type.");
        }

        [Fact]
        public void BeJsonSerializableInto_NullPropertyValueDifferent()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "Actual value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "Actual value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = "Not null",
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.null_property: Expected property to be 'String' type instead of 'Null' type.");
        }

        [Fact]
        public void BeJsonSerializableInto_InnerObjectDifferentValue()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "The string value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject()
                {
                    InnerStringProperty = "Inner string value",
                },
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "The string value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = (string)null,
                    inner_object = new
                    {
                        inner_string_property = "Other inner string value",
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.inner_object.inner_string_property: Expected 'Other inner string value' instead of 'Inner string value'.");
        }

        [Fact]
        public void BeJsonSerializableInto_CollectionDifferentItemValue()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "The string value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject()
                {
                    InnerStringProperty = "Inner string value",
                },
                CollectionInt32 = new List<int>
                {
                    10,
                    20,
                },
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "The string value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = (string)null,
                    inner_object = new
                    {
                        inner_string_property = "Inner string value",
                    },
                    collection_int = new[]
                    {
                        10,
                        1234,
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.collection_int[1]: Expected '1234' instead of '20'.");
        }

        [Fact]
        public void BeJsonSerializableInto_CollectionMissingItemDotNetClass()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "The string value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject()
                {
                    InnerStringProperty = "Inner string value",
                },
                CollectionInt32 = new List<int>
                {
                    10,
                },
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "The string value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = (string)null,
                    inner_object = new
                    {
                        inner_string_property = "Inner string value",
                    },
                    collection_int = new[]
                    {
                        10,
                        1234,
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.collection_int: Expected 2 item(s) but found 1.");
        }

        [Fact]
        public void BeJsonSerializableInto_CollectionMissingItemJson()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "The string value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject()
                {
                    InnerStringProperty = "Inner string value",
                },
                CollectionInt32 = new List<int>
                {
                    10,
                    20,
                },
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "The string value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = (string)null,
                    inner_object = new
                    {
                        inner_string_property = "Inner string value",
                    },
                    collection_int = new[]
                    {
                        10,
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.collection_int: Expected 1 item(s) but found 2.");
        }

        [Fact]
        public void BeJsonSerializableInto_PropertyTypeDifferent()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "Actual value",
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = new { },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.string_property: Expected property to be 'Object' type instead of 'String' type.");
        }

        [Fact]
        public void BeJsonSerializableInto_PropertyNameDifferent()
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "Actual value",
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property_other_name = "Different value",
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$: Expected property with the 'string_property_other_name' name but found 'string_property' instead.");
        }

        [Fact]
        public void BeJsonSerializableInto_DotNetPropertyMissing()
        {
            var json = new JsonSerializableClassInnerObject()
            {
                InnerStringProperty = "Actual value",
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    inner_string_property = "Actual value",
                    new_property = "Should not exists",
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$: Expected 'new_property' property but found no property.");
        }

        [Fact]
        public void BeJsonSerializableInto_JsonPropertyMissing()
        {
            var json = new JsonSerializableClassInnerObject()
            {
                InnerStringProperty = "Actual value",
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$: Expected no property but found 'inner_string_property' property.");
        }

        [Fact]
        public void BeJsonSerializableInto_NullSubject()
        {
            var json = (object)null;

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new { });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("A JSON object was expected.");
        }

        [Fact]
        public void BeJsonSerializableInto_NullSubjectAndNullExpected()
        {
            var json = (object)null;

            json.Should().BeJsonSerializableInto(null);
        }

        [Fact]
        public void BeJsonDeserializableInto()
        {
            var json = new
            {
                string_property = "The string value",
                int32_property = 1234,
                boolean_property = true,
                null_property = (string)null,
                inner_object = new
                {
                    inner_string_property = "Inner string value",
                },
                collection_int = new[]
                {
                    10,
                    20,
                },
                collection_object = new[]
                {
                    new
                    {
                        inner_string_property = "Inner object 1",
                    },
                    new
                    {
                        inner_string_property = "Inner object 2",
                    },
                },
            };

            var expectedObject = new JsonSerializableClass()
            {
                StringProperty = "The string value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject()
                {
                    InnerStringProperty = "Inner string value",
                },
                CollectionInt32 = new List<int>
                {
                    10,
                    20,
                },
                CollectionObjects = new List<JsonSerializableClassInnerObject>()
                {
                    new JsonSerializableClassInnerObject()
                    {
                        InnerStringProperty = "Inner object 1",
                    },
                    new JsonSerializableClassInnerObject()
                    {
                        InnerStringProperty = "Inner object 2",
                    },
                },
            };

            json.Should().BeJsonDeserializableInto(expectedObject);
        }

        [Fact]
        public void BeJsonDeserializableInto_WithSubjectNullAndExpectedNull()
        {
            var json = (object)null;

            var expectedObject = (JsonSerializableClass)null;

            json.Should().BeJsonDeserializableInto(expectedObject);
        }

        private class JsonSerializableClass
        {
            [JsonPropertyName("string_property")]
            public string StringProperty { get; set; }

            [JsonPropertyName("int32_property")]
            public int Int32Property { get; set; }

            [JsonPropertyName("boolean_property")]
            public bool BooleanProperty { get; set; }

            [JsonPropertyName("null_property")]
            public string NullProperty { get; set; }

            [JsonPropertyName("inner_object")]
            public JsonSerializableClassInnerObject InnerObject { get; set; }

            [JsonPropertyName("collection_int")]
            public List<int> CollectionInt32 { get; set; }

            [JsonPropertyName("collection_object")]
            public List<JsonSerializableClassInnerObject> CollectionObjects { get; set; }
        }

        private class JsonSerializableClassInnerObject
        {
            [JsonPropertyName("inner_string_property")]
            public string InnerStringProperty { get; set; }
        }
    }
}