//-----------------------------------------------------------------------
// <copyright file="JsonElementComparer.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    using System.Collections;
    using System.Reflection;
    using System.Text.Json;

    internal static class JsonElementComparer
    {
        public static IReadOnlyList<string> CompareWithDocument(JsonDocument document, JsonDocument expected)
        {
            return CompareWithElement(document.RootElement, expected.RootElement, "$");
        }

        public static IReadOnlyList<string> CompareWithElement(JsonElement element, JsonElement expected, string initialPath)
        {
            var errors = new List<string>();

            var path = new Stack<string>();

            path.Push(initialPath);

            CompareWithElement(element, expected, path, errors);

            path.Pop();

            return errors;
        }

        public static IReadOnlyList<string> CompareWithObject(JsonDocument document, object? expected)
        {
            return CompareWithObject(document.RootElement, expected, "$");
        }

        public static IReadOnlyList<string> CompareWithObject(JsonElement element, object? expected, string initialPath)
        {
            var errors = new List<string>();

            var path = new Stack<string>();

            path.Push(initialPath);

            CompareWithObject(element, expected, path, errors);

            path.Pop();

            return errors;
        }

        private static void CompareWithElement(JsonElement element, JsonElement expected, Stack<string> path, List<string> errors)
        {
            if (element.ValueKind != expected.ValueKind)
            {
                errors.Add($"{GetPath(path)}: Expected property to be '{expected.ValueKind}' type instead of '{element.ValueKind}' type.");
                return;
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                var value = element.GetString();
                var expectedStringValue = expected.GetString();

                if (value != expectedStringValue)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expected}' instead of '{value}'.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.Number)
            {
                var value = element.GetDouble();
                var expectedDoubleValue = expected.GetDouble();

                if (value != expectedDoubleValue)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expected}' instead of '{value}'.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                var expectedPropertyEnumerator = expected.EnumerateObject();

                foreach (var property in element.EnumerateObject())
                {
                    if (!expectedPropertyEnumerator.MoveNext())
                    {
                        errors.Add($"{GetPath(path)}: Expected no property but found '{property.Name}' property.");
                        continue;
                    }

                    var expectedProperty = expectedPropertyEnumerator.Current;

                    if (property.Name != expectedProperty.Name)
                    {
                        errors.Add($"{GetPath(path)}: Expected property with the '{expectedProperty.Name}' name but found '{property.Name}' instead.");
                        continue;
                    }

                    path.Push("." + property.Name);

                    var expectedValueOfProperty = expectedProperty.Value;

                    CompareWithElement(property.Value, expectedValueOfProperty, path, errors);

                    path.Pop();
                }

                if (expectedPropertyEnumerator.MoveNext())
                {
                    errors.Add($"{GetPath(path)}: Expected '{expectedPropertyEnumerator.Current.Name}' property but found no property.");
                    return;
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

                        errors.Add($"{GetPath(path)}: Expected {index} item(s) but found {actualCount}.");
                        continue;
                    }

                    var expectedItem = expectedArrayEnumerator.Current;

                    path.Push($"[{index}]");

                    CompareWithElement(item, expectedItem, path, errors);

                    path.Pop();

                    index++;
                }

                if (expectedArrayEnumerator.MoveNext())
                {
                    var expectedCount = expected.GetArrayLength();

                    errors.Add($"{GetPath(path)}: Expected {expectedCount} item(s) but found {index}.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.True)
            {
                var expectedBooleanValue = expected.GetBoolean();

                if (expectedBooleanValue != true)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expectedBooleanValue}' instead of '{true}'.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.False)
            {
                var expectedBooleanValue = expected.GetBoolean();

                if (expectedBooleanValue != false)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expectedBooleanValue}' instead of '{false}'.");
                    return;
                }
            }
        }

        private static void CompareWithObject(JsonElement element, object? expected, Stack<string> path, List<string> errors)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                var value = element.GetString();

                if (expected is not string expectedStringValue)
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }

                if (value != expectedStringValue)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expected}' instead of '{value}'.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.Number)
            {
                var value = element.GetDouble();

                if (expected == null || !ReflectionHelper.IsNumeric(expected))
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }

                var expectedDoubleValue = Convert.ToDouble(expected);

                if (value != expectedDoubleValue)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expected}' instead of '{value}'.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                if (expected is null || expected is string || ReflectionHelper.IsNumeric(expected) || expected is IEnumerable || expected is bool)
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }

                var expectedPropertyEnumerator = expected.GetType().GetProperties().Cast<PropertyInfo>().GetEnumerator();

                foreach (var property in element.EnumerateObject())
                {
                    if (!expectedPropertyEnumerator.MoveNext())
                    {
                        errors.Add($"{GetPath(path)}: Expected no property but found '{property.Name}' property.");
                        continue;
                    }

                    var expectedProperty = expectedPropertyEnumerator.Current;

                    if (property.Name != expectedProperty.Name)
                    {
                        errors.Add($"{GetPath(path)}: Expected property with the '{expectedProperty.Name}' name but found '{property.Name}' instead.");
                        continue;
                    }

                    path.Push("." + property.Name);

                    var expectedValueOfProperty = expectedProperty.GetValue(expected);

                    CompareWithObject(property.Value, expectedValueOfProperty, path, errors);

                    path.Pop();
                }

                if (expectedPropertyEnumerator.MoveNext())
                {
                    errors.Add($"{GetPath(path)}: Expected '{expectedPropertyEnumerator.Current.Name}' property but found no property.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                if (expected is string || expected is not IEnumerable expectedEnumerableValue)
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }

                var expectedArrayEnumerator = expectedEnumerableValue.GetEnumerator();
                var index = 0;

                foreach (var item in element.EnumerateArray())
                {
                    if (!expectedArrayEnumerator.MoveNext())
                    {
                        var actualCount = element.EnumerateArray().Count();

                        errors.Add($"{GetPath(path)}: Expected {index} item(s) but found {actualCount}.");
                        continue;
                    }

                    var expectedItem = expectedArrayEnumerator.Current;

                    path.Push($"[{index}]");

                    CompareWithObject(item, expectedItem, path, errors);

                    path.Pop();

                    index++;
                }

                if (expectedArrayEnumerator.MoveNext())
                {
                    var expectedCount = ((IEnumerable)expected).Cast<object>().Count();

                    errors.Add($"{GetPath(path)}: Expected {expectedCount} item(s) but found {index}.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.True)
            {
                if (expected is not bool expectedBooleanValue)
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }

                if (expectedBooleanValue != true)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expected}' instead of '{true}'.");
                    return;
                }
            }
            else if (element.ValueKind == JsonValueKind.False)
            {
                if (expected is not bool expectedBooleanValue)
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }

                if (expectedBooleanValue != false)
                {
                    errors.Add($"{GetPath(path)}: Expected '{expected}' instead of '{false}'.");
                    return;
                }
            }
            else
            {
                if (expected is not null)
                {
                    errors.Add($"{GetPath(path)}: Expected property to be '{ReflectionHelper.GetJsonKind(expected)}' type instead of '{element.ValueKind}' type.");
                    return;
                }
            }
        }

        private static string GetPath(IEnumerable<string> path)
        {
            return string.Concat(path.Reverse());
        }
    }
}
