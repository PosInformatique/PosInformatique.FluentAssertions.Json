//-----------------------------------------------------------------------
// <copyright file="JsonFluentAssertionsExtensionsTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace FluentAssertions.Json.Tests
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Serialization;
    using PosInformatique.FluentAssertions.Json;
    using Xunit.Sdk;

    public class JsonFluentAssertionsExtensionsTest
    {
        private enum EnumTest
        {
            A,
            B = 100,
            C = 200,
        }

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
                .WithMessage($"$.boolean_property: Expected '{expectedValueString}' instead of '{actualValueString}'.");
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
        public void BeJsonSerializableInto_PropertyTypeDifferent_String()
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

        [Theory]
        [InlineData("Expected string", "String")]
        [InlineData(null, "Null")]
        public void BeJsonSerializableInto_PropertyTypeDifferent_Number(object expectedValue, string expectedTypeMessage)
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "String value",
                Int32Property = 1234,
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "String value",
                    int32_property = expectedValue,
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.int32_property: Expected property to be '{expectedTypeMessage}' type instead of 'Number' type.");
        }

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public void BeJsonSerializableInto_PropertyTypeDifferent_Boolean(bool value, string insteadOfMessageString)
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "String value",
                Int32Property = 1234,
                BooleanProperty = value,
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "String value",
                    int32_property = 1234,
                    boolean_property = new { },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.boolean_property: Expected property to be 'Object' type instead of '{insteadOfMessageString}' type.");
        }

        [Theory]
        [InlineData("String value", "String")]
        [InlineData(12.34, "Number")]
        [InlineData(1234, "Number")]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        [InlineData(null, "Null")]
        public void BeJsonSerializableInto_PropertyTypeDifferent_Object(object value, string expectedKindMessage)
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "String value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject(),
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "String value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = (int?)null,
                    inner_object = value,
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.inner_object: Expected property to be '{expectedKindMessage}' type instead of 'Object' type.");
        }

        [Theory]
        [InlineData("String value", "String")]
        [InlineData(12.34, "Number")]
        [InlineData(1234, "Number")]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        [InlineData(null, "Null")]
        public void BeJsonSerializableInto_PropertyTypeDifferent_Array(object value, string expectedKindMessage)
        {
            var json = new JsonSerializableClass()
            {
                StringProperty = "String value",
                Int32Property = 1234,
                BooleanProperty = true,
                NullProperty = null,
                InnerObject = new JsonSerializableClassInnerObject() { InnerStringProperty = "Inner string" },
                CollectionInt32 = new List<int> { 1, 2 },
            };

            var act = () =>
            {
                json.Should().BeJsonSerializableInto(new
                {
                    string_property = "String value",
                    int32_property = 1234,
                    boolean_property = true,
                    null_property = (int?)null,
                    inner_object = new { inner_string_property = "Inner string" },
                    collection_int = value,
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.collection_int: Expected property to be '{expectedKindMessage}' type instead of 'Array' type.");
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
        public void BeJsonSerializableInto_WithNoSpecificOptions()
        {
            var json = new JsonSerializableClassWithEnum()
            {
                Int32Property = 10,
                EnumProperty = EnumTest.B,
            };

            json.Should().BeJsonSerializableInto(new
            {
                int32_property = 10,
                enum_property = 100,
            });
        }

        [Fact]
        public void BeJsonSerializableInto_WithSpecificOptions()
        {
            var json = new JsonSerializableClassWithEnum()
            {
                Int32Property = 10,
                EnumProperty = EnumTest.B,
            };

            var options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                },
            };

            json.Should().BeJsonSerializableInto(
                new
                {
                    int32_property = 10,
                    enum_property = "B",
                },
                options);
        }

        [Fact]
        public void BeJsonSerializableInto_WithDefaultOptionsChanged()
        {
            var oldConfig = FluentAssertionsJson.Configuration;

            FluentAssertionsJson.Configuration = new FluentAssertionsJsonConfiguration();
            FluentAssertionsJson.Configuration.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            try
            {
                var json = new JsonSerializableClassWithEnum()
                {
                    Int32Property = 10,
                    EnumProperty = EnumTest.B,
                };

                json.Should().BeJsonSerializableInto(new
                {
                    int32_property = 10,
                    enum_property = "B",
                });
            }
            finally
            {
                FluentAssertionsJson.Configuration = oldConfig;
            }
        }

        [Fact]
        public void BeJsonSerializableInto_WithDelegateOptions()
        {
            var dummyConverter = new DummyJsonConverter();

            var oldConfig = FluentAssertionsJson.Configuration;

            FluentAssertionsJson.Configuration = new FluentAssertionsJsonConfiguration();
            FluentAssertionsJson.Configuration.JsonSerializerOptions.Converters.Add(dummyConverter);

            try
            {
                var json = new JsonSerializableClassWithEnum()
                {
                    Int32Property = 10,
                    EnumProperty = EnumTest.B,
                };

                json.Should().BeJsonSerializableInto(
                    new
                    {
                        int32_property = 10,
                        enum_property = "B",
                    },
                    opt =>
                    {
                        opt.Converters.Should().Contain(dummyConverter);
                        opt.Converters.Remove(dummyConverter);
                        opt.Converters.Add(new JsonStringEnumConverter());
                    });
            }
            finally
            {
                FluentAssertionsJson.Configuration = oldConfig;
            }
        }

        [Fact]
        public void BeJsonSerializableInto_WithPolymorphism()
        {
            var point = new ThreeDimensionalPoint()
            {
                X = 1,
                Y = 2,
                Z = 3,
            };

            point.Should().BeJsonSerializableInto<BasePoint>(
                new
                {
                    myType = "3d",
                    X = 1,
                    Y = 2,
                    Z = 3,
                });
        }

        [Fact]
        public void BeJsonSerializableInto_WithPolymorphism_WithOptions()
        {
            var point = new ThreeDimensionalPoint()
            {
                X = 1,
                Y = 2,
                Z = 3,
            };

            point.Should().BeJsonSerializableInto<BasePoint>(
                new
                {
                    myType = "3d",
                    x = 1,
                    y = 2,
                    z = 3,
                },
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
        }

        [Fact]
        public void BeJsonSerializableInto_WithPolymorphism_AndConfigureOptions()
        {
            var point = new ThreeDimensionalPoint()
            {
                X = 1,
                Y = 2,
                Z = 3,
            };

            point.Should().BeJsonSerializableInto<BasePoint>(
                new
                {
                    myType = "3d",
                    x = 1,
                    y = 2,
                    z = 3,
                },
                opt =>
                {
                    opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        }

        [Fact]
        public void BeJsonSerializableInto_WithPolymorphism_WithWrongInheritance()
        {
            var point = new ThreeDimensionalPoint()
            {
                X = 1,
                Y = 2,
                Z = 3,
            };

            var act = () =>
            {
                point.Should().BeJsonSerializableInto<string>(
                    new
                    {
                        myType = "3d",
                    });
            };

            act.Should().ThrowExactly<ArgumentException>()
                .WithMessage("The 'System.String' class is not a base class of the 'FluentAssertions.Json.Tests.JsonFluentAssertionsExtensionsTest+ThreeDimensionalPoint' type. (Parameter 'TBase')")
                .And.ParamName.Should().Be("TBase");
        }

        [Theory]
        [InlineData(1234)]
        [InlineData("The string")]
        public void BeJsonSerializableInto_WithObjectProperty(object value)
        {
            var obj = new ClassWithObjectProperty()
            {
                Inner = new InnerClassWithObjectProperty()
                {
                    ObjectProperty = value,
                },
            };

            obj.Should().BeJsonSerializableInto(
                new
                {
                    inner = new
                    {
                        object_property = value,
                    },
                    collection_of_inner = (string)null,
                });
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

        [Fact]
        public void BeJsonDeserializableInto_WithNoOptions()
        {
            var json = new
            {
                int32_property = 10,
                enum_property = 100,
            };

            json.Should().BeJsonDeserializableInto(
                new JsonSerializableClassWithEnum()
                {
                    Int32Property = 10,
                    EnumProperty = EnumTest.B,
                });
        }

        [Fact]
        public void BeJsonDeserializableInto_WithSpecificOptions()
        {
            var json = new
            {
                int32_property = 10,
                enum_property = "B",
            };

            var options = new JsonSerializerOptions()
            {
                Converters =
                {
                    new JsonStringEnumConverter(),
                },
            };

            json.Should().BeJsonDeserializableInto(
                new JsonSerializableClassWithEnum()
                {
                    Int32Property = 10,
                    EnumProperty = EnumTest.B,
                },
                options);
        }

        [Fact]
        public void BeJsonDeserializableInto_WithDefaultGlobalOptions()
        {
            var oldConfig = FluentAssertionsJson.Configuration;

            FluentAssertionsJson.Configuration = new FluentAssertionsJsonConfiguration();
            FluentAssertionsJson.Configuration.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            try
            {
                var json = new
                {
                    int32_property = 10,
                    enum_property = "B",
                };

                json.Should().BeJsonDeserializableInto(
                    new JsonSerializableClassWithEnum()
                    {
                        Int32Property = 10,
                        EnumProperty = EnumTest.B,
                    });
            }
            finally
            {
                FluentAssertionsJson.Configuration = oldConfig;
            }
        }

        [Fact]
        public void BeJsonDeserializableInto_WithDelegateOptions()
        {
            var dummyConverter = new DummyJsonConverter();

            var oldConfig = FluentAssertionsJson.Configuration;

            FluentAssertionsJson.Configuration = new FluentAssertionsJsonConfiguration();
            FluentAssertionsJson.Configuration.JsonSerializerOptions.Converters.Add(dummyConverter);

            try
            {
                var json = new
                {
                    int32_property = 10,
                    enum_property = "B",
                };

                json.Should().BeJsonDeserializableInto(
                    new JsonSerializableClassWithEnum()
                    {
                        Int32Property = 10,
                        EnumProperty = EnumTest.B,
                    },
                    opt =>
                    {
                        opt.Converters.Should().Contain(dummyConverter);
                        opt.Converters.Remove(dummyConverter);
                        opt.Converters.Add(new JsonStringEnumConverter());
                    });
            }
            finally
            {
                FluentAssertionsJson.Configuration = oldConfig;
            }
        }

        [Fact]
        public void BeJsonDeserializableInto_WithAnonymousArray()
        {
            var json = new[]
            {
                new
                {
                    X = 1,
                    Y = 2,
                },
                new
                {
                    X = 3,
                    Y = 4,
                },
            };

            json.Should().BeJsonDeserializableInto(
                new[]
                {
                    new BasePoint() { X = 1, Y = 2 },
                    new BasePoint() { X = 3, Y = 4 },
                });
        }

        [Fact]
        public void BeJsonDeserializableInto_WithAnonymousArray_WithOptions()
        {
            var json = new[]
            {
                new
                {
                    x = 1,
                    y = 2,
                },
                new
                {
                    x = 3,
                    y = 4,
                },
            };

            json.Should().BeJsonDeserializableInto(
                new[]
                {
                    new BasePoint() { X = 1, Y = 2 },
                    new BasePoint() { X = 3, Y = 4 },
                },
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
        }

        [Fact]
        public void BeJsonDeserializableInto_WithAnonymousArray_WithOptionsConfigure()
        {
            var json = new[]
            {
                new
                {
                    x = 1,
                    y = 2,
                },
                new
                {
                    x = 3,
                    y = 4,
                },
            };

            json.Should().BeJsonDeserializableInto(
                new[]
                {
                    new BasePoint() { X = 1, Y = 2 },
                    new BasePoint() { X = 3, Y = 4 },
                },
                opt =>
                {
                    opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        }

        [Theory]
        [InlineData(1234, 1234)]
        [InlineData(1234, 1234.0)]
        [InlineData(1234.0, 1234)]
        [InlineData("The string", "The string")]
        [InlineData(12.32, 12.32)]
        public void BeJsonDeserializableInto_WithObjectProperty(object value, object expectedValue)
        {
            var json = new
            {
                inner = new
                {
                    object_property = value,
                },
                collection_of_inner = new[]
                {
                    new
                    {
                        object_property = value,
                    },
                    new
                    {
                        object_property = value,
                    },
                },
            };

            json.Should().BeJsonDeserializableInto(new ClassWithObjectProperty()
            {
                Inner = new InnerClassWithObjectProperty()
                {
                    ObjectProperty = expectedValue,
                },
                InnerCollection = new List<InnerClassWithObjectProperty>()
                {
                    new InnerClassWithObjectProperty()
                    {
                        ObjectProperty = expectedValue,
                    },
                    new InnerClassWithObjectProperty()
                    {
                        ObjectProperty = expectedValue,
                    },
                },
            });
        }

        [Theory]
        [InlineData(1234)]
        [InlineData(12.34)]
        public void BeJsonDeserializableInto_WithObjectProperty_Number_Different(object value)
        {
            var json = new
            {
                inner = new
                {
                    object_property = value,
                },
            };

            var act = () =>
            {
                json.Should().BeJsonDeserializableInto(new ClassWithObjectProperty()
                {
                    Inner = new InnerClassWithObjectProperty()
                    {
                        ObjectProperty = 8888,
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.inner.object_property: Expected '8888' instead of '{value}'.");
        }

        [Theory]
        [InlineData(1234)]
        [InlineData(12.34)]
        public void BeJsonDeserializableInto_WithObjectProperty_Number_WrongType(object value)
        {
            var json = new
            {
                inner = new
                {
                    object_property = value,
                },
            };

            var act = () =>
            {
                json.Should().BeJsonDeserializableInto(new ClassWithObjectProperty()
                {
                    Inner = new InnerClassWithObjectProperty()
                    {
                        ObjectProperty = "Other type",
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage($"$.inner.object_property: Expected property to be 'String' type instead of 'Number' type.");
        }

        [Fact]
        public void BeJsonDeserializableInto_WithObjectProperty_String_Different()
        {
            var json = new
            {
                inner = new
                {
                    object_property = "The actual string",
                },
            };

            var act = () =>
            {
                json.Should().BeJsonDeserializableInto(new ClassWithObjectProperty()
                {
                    Inner = new InnerClassWithObjectProperty()
                    {
                        ObjectProperty = "The expected string",
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.inner.object_property: Expected 'The expected string' instead of 'The actual string'.");
        }

        [Fact]
        public void BeJsonDeserializableInto_WithObjectProperty_String_WrongType()
        {
            var json = new
            {
                inner = new
                {
                    object_property = "The actual string",
                },
            };

            var act = () =>
            {
                json.Should().BeJsonDeserializableInto(new ClassWithObjectProperty()
                {
                    Inner = new InnerClassWithObjectProperty()
                    {
                        ObjectProperty = 1234,
                    },
                });
            };

            act.Should().ThrowExactly<XunitException>()
                .WithMessage("$.inner.object_property: Expected property to be 'Number' type instead of 'String' type.");
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
            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public List<int> CollectionInt32 { get; set; }

            [JsonPropertyName("collection_object")]
            public List<JsonSerializableClassInnerObject> CollectionObjects { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public string IgnoredProperty
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
        }

        private class JsonSerializableClassInnerObject
        {
            [JsonPropertyName("inner_string_property")]
            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public string InnerStringProperty { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public string InnerIgnoredProperty
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
        }

        private class JsonSerializableClassWithEnum
        {
            [JsonPropertyName("int32_property")]
            public int Int32Property { get; set; }

            [JsonPropertyName("enum_property")]
            public EnumTest EnumProperty { get; set; }
        }

        private class DummyJsonConverter : JsonConverter<string>
        {
            public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        [JsonPolymorphic(TypeDiscriminatorPropertyName = "myType")]
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

        private class ClassWithObjectProperty
        {
            [JsonPropertyName("inner")]
            public InnerClassWithObjectProperty Inner { get; set; }

            [JsonPropertyName("collection_of_inner")]
            public List<InnerClassWithObjectProperty> InnerCollection { get; set; }
        }

        private class InnerClassWithObjectProperty
        {
            [JsonPropertyName("object_property")]
            public object ObjectProperty { get; set; }
        }
    }
}