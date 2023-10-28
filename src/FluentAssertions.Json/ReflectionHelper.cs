//-----------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    using System.Collections;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal static class ReflectionHelper
    {
        private static readonly Type[] NumericTypes = new[] { typeof(int), typeof(double) };

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

        public static JsonValueKind GetJsonKind(object? value)
        {
            if (value is null)
            {
                return JsonValueKind.Null;
            }

            if (value is string)
            {
                return JsonValueKind.String;
            }

            if (IsNumeric(value))
            {
                return JsonValueKind.Number;
            }

            if (value is IEnumerable)
            {
                return JsonValueKind.Array;
            }

            if (value is bool booleanValue)
            {
                if (booleanValue)
                {
                    return JsonValueKind.True;
                }

                return JsonValueKind.False;
            }

            return JsonValueKind.Object;
        }

        public static bool IsNumeric(object value)
        {
            var type = value.GetType();

            if (NumericTypes.Contains(type))
            {
                return true;
            }

            return false;
        }
    }
}
