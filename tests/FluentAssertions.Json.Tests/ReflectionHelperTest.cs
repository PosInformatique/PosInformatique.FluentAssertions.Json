//-----------------------------------------------------------------------
// <copyright file="ReflectionHelperTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json.Tests.Tests
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using global::FluentAssertions;

    public class ReflectionHelperTest
    {
        [Fact]
        public void GetJsonPropertyName()
        {
            ReflectionHelper.GetJsonPropertyName(typeof(ObjectWithProperties).GetProperty("Property1")).Should().Be("Property1");
            ReflectionHelper.GetJsonPropertyName(typeof(ObjectWithProperties).GetProperty("Property2")).Should().Be("property_2");
        }

        [Fact]
        public void GetJsonPath()
        {
            var @object = new RootObject()
            {
                PropertyRoot = new RootObject()
                {
                    PropertyRoot = new RootObject()
                    {
                        InnerObject = new ObjectWithProperties(),
                    },
                },
                InnerObject = new InheritedObjectWithProperties(),
                CollectionExplicitName = new Dictionary<string, ObjectWithProperties>
                {
                    { "xxx", new ObjectWithProperties() },
                    { "yyy", new ObjectWithProperties() },
                },
                CollectionImplicitName = new List<ObjectWithProperties>
                {
                    new ObjectWithProperties(),
                    new ObjectWithProperties(),
                },
            };

            ReflectionHelper.GetJsonPath(@object, "PropertyRoot.PropertyRoot.InnerObject.Property1").Should().Be("$.root.root.InnerObject.Property1");
            ReflectionHelper.GetJsonPath(@object, "PropertyRoot.PropertyRoot.InnerObject.Property2").Should().Be("$.root.root.InnerObject.property_2");
            ReflectionHelper.GetJsonPath(@object, "PropertyRoot").Should().Be("$.root");

            ReflectionHelper.GetJsonPath(@object, "InnerObject.Property1").Should().Be("$.InnerObject.Property1");
            ReflectionHelper.GetJsonPath(@object, "InnerObject.Property2").Should().Be("$.InnerObject.property_2");
            ReflectionHelper.GetJsonPath(@object, "InnerObject.Property3").Should().Be("$.InnerObject.Property3");
            ReflectionHelper.GetJsonPath(@object, "InnerObject.Property4").Should().Be("$.InnerObject.property_4");

            ReflectionHelper.GetJsonPath(@object, "CollectionImplicitName[0].Property1").Should().Be("$.CollectionImplicitName[0].Property1");
            ReflectionHelper.GetJsonPath(@object, "CollectionImplicitName[1].Property1").Should().Be("$.CollectionImplicitName[1].Property1");
            ReflectionHelper.GetJsonPath(@object, "CollectionExplicitName[xxx].Property2").Should().Be("$.collection_explicit_name[xxx].property_2");
            ReflectionHelper.GetJsonPath(@object, "CollectionExplicitName[yyy].Property2").Should().Be("$.collection_explicit_name[yyy].property_2");
        }

        [Fact]
        public void GetJsonKind()
        {
            ReflectionHelper.GetJsonKind("The string").Should().Be(JsonValueKind.String);

            ReflectionHelper.GetJsonKind(1234).Should().Be(JsonValueKind.Number);
            ReflectionHelper.GetJsonKind(12.34).Should().Be(JsonValueKind.Number);

            ReflectionHelper.GetJsonKind(new[] { 1 }).Should().Be(JsonValueKind.Array);
            ReflectionHelper.GetJsonKind(new List<int> { 1 }).Should().Be(JsonValueKind.Array);

            ReflectionHelper.GetJsonKind(new { }).Should().Be(JsonValueKind.Object);

            ReflectionHelper.GetJsonKind(true).Should().Be(JsonValueKind.True);
            ReflectionHelper.GetJsonKind(false).Should().Be(JsonValueKind.False);
        }

        [Fact]
        public void IsNumeric()
        {
            ReflectionHelper.IsNumeric(1234).Should().BeTrue();
            ReflectionHelper.IsNumeric(12.34).Should().BeTrue();

            ReflectionHelper.IsNumeric("Not numeric").Should().BeFalse();
            ReflectionHelper.IsNumeric(new { }).Should().BeFalse();
            ReflectionHelper.IsNumeric(new[] { 1 }).Should().BeFalse();
        }

        private class RootObject
        {
            [JsonPropertyName("root")]
            public RootObject PropertyRoot { get; set; }

            public ObjectWithProperties InnerObject { get; set; }

            public List<ObjectWithProperties> CollectionImplicitName { get; set; }

            [JsonPropertyName("collection_explicit_name")]
            public Dictionary<string, ObjectWithProperties> CollectionExplicitName { get; set; }
        }

        private class ObjectWithProperties
        {
            public string Property1 { get; set; }

            [JsonPropertyName("property_2")]
            public string Property2 { get; set; }
        }

        private class InheritedObjectWithProperties : ObjectWithProperties
        {
            public string Property3 { get; set; }

            [JsonPropertyName("property_4")]
            public string Property4 { get; set; }
        }
    }
}