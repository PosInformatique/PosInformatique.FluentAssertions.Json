//-----------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    using System.Reflection;
    using System.Text.Json.Serialization;

    internal static class ReflectionHelper
    {
        public static string GetJsonPropertyName(PropertyInfo property)
        {
            var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();

            if (jsonPropertyName is not null)
            {
                return jsonPropertyName.Name;
            }

            return property.Name;
        }

        public static string GetJsonPath(Type type, string path)
        {
            var propertyNames = path.Split('.');

            var result = new List<string>();

            foreach (var propertyName in propertyNames)
            {
                var property = type.GetProperty(propertyName);

                result.Add(GetJsonPropertyName(property));

                type = property.PropertyType;
            }

            return $"$.{string.Join(".", result)}";
        }
    }
}
