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

        public static string GetJsonPath(object instance, string path)
        {
            var propertyNames = path.Split('.');

            var result = new List<string>();

            foreach (var p in propertyNames)
            {
                var propertyName = p;

                // Extract if need an index for properties collection.
                var indexArray = propertyName.IndexOf("[");

                if (indexArray > 0)
                {
                    propertyName = p.Substring(0, indexArray);
                }

                var property = instance.GetType().GetProperty(propertyName);

                result.Add(GetJsonPropertyName(property));

                if (indexArray > 0)
                {
                    // Append the "[xxx]" index.
                    var indexString = p.Substring(indexArray);

                    result[result.Count - 1] = result[result.Count - 1] + indexString;

                    instance = property.GetValue(instance, null);

                    // Gets the item of the list
                    indexString = indexString.TrimStart("[").TrimEnd("]").ToString();
                    instance = GetListItem(instance, indexString);
                }
                else
                {
                    instance = property.GetValue(instance, null);
                }
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

        private static object GetListItem(object collection, string indexString)
        {
            var itemProperty = collection.GetType().GetProperty("Item");

            var itemType = itemProperty.GetIndexParameters()[0].ParameterType;
            var index = Convert.ChangeType(indexString, itemType);

            return itemProperty.GetValue(collection, [index]);
        }
    }
}
