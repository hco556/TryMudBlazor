using Northwind.CodeGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TryMudBlazor.Client.Generators
{

    /// <summary>
    /// Generates a simple ViewModel class source from a model type.
    /// - Excludes properties that have an [InverseProperty] attribute.
    /// - Excludes properties that are referenced by any [ForeignKey("...")] attribute on any property.
    /// - Emits nullable reference annotations (?) when ReflectionExtensions.IsNullable(property) reports true.
    /// - Emits [Required(ErrorMessage = "...")] when the source property has a RequiredAttribute (uses attribute message if present, otherwise "<Label> is required.").
    /// </summary>
    public static class ViewModelGenerator
    {
        public static string GenerateViewModelSource(Type modelType, string viewModelNamespace = "Generated.ViewModels", string viewModelName = null)
        {
            if (modelType == null) throw new ArgumentNullException(nameof(modelType));
            viewModelName ??= modelType.Name + "ViewModel";

            // collect public instance properties
            var props = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.GetIndexParameters().Length == 0) // skip indexers
                                 .ToList();
            var exludedProps = new List<PropertyInfo>();
            // 1) properties that have InverseProperty attribute -> exclude
            foreach (var p in props)
            {
                var fkAttr = p.GetCustomAttribute<InversePropertyAttribute>();
                if (fkAttr != null)
                {
                    // find the referenced FK property (if present) and exclude it
                    // InversePropertyAttribute has a "Property" value when used via string constructor in some patterns;
                    // if not available, we still exclude the navigation property itself.
                    var referencedName = fkAttr.Property;
                    if (!string.IsNullOrWhiteSpace(referencedName))
                    {
                        PropertyInfo fkProperty = modelType.GetProperty(referencedName);
                        if (fkProperty != null)
                            exludedProps.Add(fkProperty);
                    }
                    // also exclude the inverse/nav property itself
                    exludedProps.Add(p);
                }
            }

            // 2) properties referenced by ForeignKey attributes -> exclude those target names
            foreach (var p in props)
            {
                var fkAttr = p.GetCustomAttribute<ForeignKeyAttribute>();
                if (fkAttr != null && !string.IsNullOrWhiteSpace(fkAttr.Name))
                {
                    PropertyInfo fkProperty = modelType.GetProperty(fkAttr.Name);
                    if (fkProperty != null)
                        exludedProps.Add(fkProperty);

                    // also exclude the navigation property that declared the ForeignKey attribute
                    exludedProps.Add(p);
                }
            }

            var finalProps = props.Except(exludedProps).ToList();

            // Build usings (always include common ones and model's namespace)
            var usings = new HashSet<string> {
                "System",
                "System.Collections.Generic",
                "System.ComponentModel"
            };
            if (!string.IsNullOrEmpty(modelType.Namespace))
                usings.Add(modelType.Namespace);

            // If any final property requires DataAnnotations (Required), add the using so generated file compiles
            if (finalProps.Any(p => p.GetCustomAttribute<RequiredAttribute>() != null))
                usings.Add("System.ComponentModel.DataAnnotations");

            // also add namespaces used by property types
            foreach (var p in finalProps)
            {
                CollectNamespacesForType(p.PropertyType, usings);
            }

            var sb = new StringBuilder();

            // usings
            foreach (var u in usings.OrderBy(x => x))
                sb.AppendLine($"using {u};");
            sb.AppendLine();

            // namespace and class
            sb.AppendLine($"namespace {viewModelNamespace};");
            sb.AppendLine();
            sb.AppendLine($"public class {viewModelName}");
            sb.AppendLine("{");

            foreach (var p in finalProps)
            {
                // Determine label and required message
                var label = SplitPascalCase(p.Name);

                var requiredAttr = p.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttr != null)
                {
                    // Prefer the ErrorMessage set on the attribute; otherwise use a default message.
                    var rawMessage = !string.IsNullOrWhiteSpace(requiredAttr.ErrorMessage)
                        ? requiredAttr.ErrorMessage
                        : $"{label} is required.";

                    // escape quotes/backslashes for C# source string literal
                    var escaped = rawMessage.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    sb.AppendLine($"    [Required(ErrorMessage = \"{escaped}\")]");
                }

                sb.AppendLine($"    [DisplayName(\"{label}\")]");

                // Build type declaration and append '?' for nullable reference types when ReflectionExtensions reports nullable.
                var typeDecl = GetFriendlyTypeName(p.PropertyType, p);

                try
                {
                    var isNullable = p.IsNullable();
                    if (isNullable && !typeDecl.EndsWith("?"))
                    {
                        // Append '?' for nullable reference types and nullable generic/collection types
                        typeDecl += "?";
                    }
                }
                catch
                {
                    // If helper fails for any reason, fall back to the previously determined typeDecl.
                }

                sb.AppendLine($"    public {typeDecl} {p.Name} {{ get; set; }}");
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void CollectNamespacesForType(Type t, HashSet<string> collector)
        {
            if (t == null) return;

            if (!string.IsNullOrEmpty(t.Namespace))
                collector.Add(t.Namespace);

            if (t.IsGenericType)
            {
                foreach (var arg in t.GetGenericArguments())
                    CollectNamespacesForType(arg, collector);
            }
        }


        private static string GetFriendlyTypeName(Type t, PropertyInfo propForNullableCheck = null)
        {
            if (t == null) return "object";

            // Nullable<T> for value types
            var underlying = Nullable.GetUnderlyingType(t);
            if (underlying != null)
            {
                return GetFriendlyTypeName(underlying) + "?";
            }

            // arrays
            if (t.IsArray)
            {
                return GetFriendlyTypeName(t.GetElementType()) + "[]";
            }

            // generics (e.g., ICollection<T>)
            if (t.IsGenericType)
            {
                var def = t.GetGenericTypeDefinition();
                var name = def.Name;
                var backtick = name.IndexOf('`');
                if (backtick > 0) name = name.Substring(0, backtick);

                var args = t.GetGenericArguments().Select(arg => GetFriendlyTypeName(arg));
                return $"{name}<{string.Join(", ", args)}>";
            }

            // map common CLR types to C# aliases
            if (t == typeof(int)) return "int";
            if (t == typeof(long)) return "long";
            if (t == typeof(short)) return "short";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(string)) return "string";
            if (t == typeof(decimal)) return "decimal";
            if (t == typeof(double)) return "double";
            if (t == typeof(float)) return "float";
            if (t == typeof(byte[])) return "byte[]";
            if (t == typeof(DateTime)) return "DateTime";
            if (t == typeof(object)) return "object";

            // reference-type nullability (C# 8+ nullable annotations) - if helper exists use it
            try
            {
                if (propForNullableCheck != null && propForNullableCheck.IsNullable())
                {
                    // append ? for nullable reference types
                    return t.Name + "?";
                }
            }
            catch
            {
                // ignore helper failures and continue
            }

            // default: use the simple type name
            return t.Name;
        }

        private static string SplitPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new StringBuilder();
            sb.Append(input[0]);
            for (int i = 1; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsUpper(c) && !char.IsWhiteSpace(input[i - 1]))
                    sb.Append(' ');
                sb.Append(c);
            }
            return sb.ToString();
        }
    }

}
