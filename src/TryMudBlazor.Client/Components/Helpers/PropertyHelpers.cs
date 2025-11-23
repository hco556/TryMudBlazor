using System.ComponentModel;
using System.Reflection;

namespace Northwind.CodeGenerator.Helpers
{
    public static class PropertyHelpers
    {
        public static T? GetAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>();
        }

        public static string? GetDisplayName(this PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }
    }
}
