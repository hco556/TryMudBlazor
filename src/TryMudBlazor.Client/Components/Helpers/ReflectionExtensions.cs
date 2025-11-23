using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Northwind.CodeGenerator.Helpers
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Determines if a property on a given class type is a collection,
        /// and returns the element type if it is.
        /// </summary>
        public static Type? GetCollectionElementType(Type classType, string propertyName)
        {
            // Find the property by name
            var property = classType.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new ArgumentException($"Property '{propertyName}' not found on {classType.Name}");

            var propertyType = property.PropertyType;

            // Handle arrays directly
            if (propertyType.IsArray)
                return propertyType.GetElementType();

            // Handle generic collections (ICollection<T>, List<T>, etc.)
            if (propertyType.IsGenericType)
            {
                var genericDef = propertyType.GetGenericTypeDefinition();
                if (typeof(IEnumerable<>).IsAssignableFrom(genericDef) ||
                    propertyType.GetInterfaces().Any(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    return propertyType.GetGenericArguments()[0];
                }
            }

            // Handle non-generic collections (like ICollection)
            if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                return typeof(object); // fallback if element type is unknown

            return null; // Not a collection
        }
        /// <summary>
        /// Returns true when the given property is declared nullable.
        /// - For value types: checks Nullable{T} (int? etc.)
        /// - For reference types: inspects the compiler-generated NullableAttribute / NullableContextAttribute metadata emitted for C# 8+ nullable reference types.
        /// Note: If the project was compiled without nullable annotations (C# nullable reference types disabled),
        /// the reference types will be reported as non-nullable (false).
        /// </summary>
        public static bool IsNullable(this System.Reflection.PropertyInfo? property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            var propertyType = property.PropertyType;

            // Value types: only nullable if Nullable<T>
            if (propertyType.IsValueType)
                return Nullable.GetUnderlyingType(propertyType) != null;

            // Reference types: inspect NullableAttribute on the property, its getter return parameter,
            // then fall back to NullableContext on the declaring type / assembly / module.
            // See: https://docs.microsoft.com/dotnet/csharp/nullable-metadata
            const string NullableAttributeFullName = "System.Runtime.CompilerServices.NullableAttribute";
            const string NullableContextAttributeFullName = "System.Runtime.CompilerServices.NullableContextAttribute";

            // 1) Check NullableAttribute on the property itself
            var customAttrs = property.CustomAttributes;
            var nullableAttr = customAttrs.FirstOrDefault(a => a.AttributeType.FullName == NullableAttributeFullName);
            if (TryReadNullableAttribute(nullableAttr, out bool isNullable))
                return isNullable;

            // 2) Check NullableAttribute on the getter return parameter (applies for auto-properties)
            var getMethod = property.GetMethod;
            if (getMethod != null)
            {
                var returnParamAttr = getMethod.ReturnParameter?.CustomAttributes
                    .FirstOrDefault(a => a.AttributeType.FullName == NullableAttributeFullName);
                if (TryReadNullableAttribute(returnParamAttr, out isNullable))
                    return isNullable;
            }

            // 3) Check NullableContextAttribute on the declaring type
            var declaringType = property.DeclaringType;
            if (declaringType != null)
            {
                var ctxAttr = declaringType.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeFullName);
                if (TryReadNullableContextAttribute(ctxAttr, out bool ctxNullable))
                    return ctxNullable;
            }

            // 4) Check NullableContextAttribute at module or assembly level
            var moduleAttr = declaringType?.Module.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeFullName);
            if (TryReadNullableContextAttribute(moduleAttr, out bool moduleNullable))
                return moduleNullable;

            var asmAttr = declaringType?.Assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == NullableContextAttributeFullName);
            if (TryReadNullableContextAttribute(asmAttr, out bool asmNullable))
                return asmNullable;

            // If nothing found, assume non-nullable reference type (this is the conservative choice).
            return false;
        }

        private static bool TryReadNullableAttribute(CustomAttributeData? attrData, out bool isNullable)
        {
            isNullable = false;
            if (attrData == null) return false;

            // NullableAttribute has either a single byte constructor arg or an array of bytes.
            if (attrData.ConstructorArguments.Count == 1)
            {
                var arg = attrData.ConstructorArguments[0];
                // pattern 1: byte -> 1 = not annotated, 2 = annotated (nullable)
                if (arg.ArgumentType == typeof(byte) && arg.Value is byte b)
                {
                    isNullable = b == 2;
                    return true;
                }

                // pattern 2: byte[] -> first element corresponds to the nullability of the target
                if (arg.ArgumentType == typeof(byte[]) && arg.Value is IReadOnlyCollection<CustomAttributeTypedArgument> arr && arr.Count > 0)
                {
                    var first = arr.First();
                    if (first.Value is byte b2)
                    {
                        isNullable = b2 == 2;
                        return true;
                    }
                }

                // sometimes reflection surfaces IList<CustomAttributeTypedArgument>
                if (arg.Value is IList<CustomAttributeTypedArgument> list && list.Count > 0 && list[0].Value is byte b3)
                {
                    isNullable = b3 == 2;
                    return true;
                }
            }

            return false;
        }

        private static bool TryReadNullableContextAttribute(CustomAttributeData? attrData, out bool isNullable)
        {
            isNullable = false;
            if (attrData == null) return false;
            // NullableContextAttribute ctor takes a single byte:
            // 1 = oblivious / not nullable, 2 = nullable context enabled
            if (attrData.ConstructorArguments.Count == 1)
            {
                var arg = attrData.ConstructorArguments[0];
                if (arg.ArgumentType == typeof(byte) && arg.Value is byte b)
                {
                    isNullable = b == 2;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Helper: returns the list of public instance properties for which IsNullable returns true.
        /// </summary>
        public static IReadOnlyList<PropertyInfo> GetNullablePublicProperties(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                       .Where(p => p.IsNullable())
                       .ToArray();
        }
    }
}
