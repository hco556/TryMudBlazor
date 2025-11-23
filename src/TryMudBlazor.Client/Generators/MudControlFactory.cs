using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryMudBlazor.Client.Generators
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class MudControlFactory
    {

        //public static TControl CreateControl<TControl, TValue>(Dictionary<string, TValue>? propertyValues = null)
        //    where TControl : new()
        //{
        //    var control = new TControl();

        //    if (propertyValues != null)
        //    {
        //        var type = typeof(TControl);

        //        foreach (var kvp in propertyValues)
        //        {
        //            var prop = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
        //            if (prop != null && prop.CanWrite)
        //            {
        //                // box the generic TValue to object for runtime checks / conversions
        //                object? rawValue = kvp.Value;

        //                // Special handling for Func<,> or delegate properties
        //                if (rawValue != null && typeof(Delegate).IsAssignableFrom(rawValue.GetType()))
        //                {
        //                    // Ensure delegate type matches property type
        //                    if (prop.PropertyType.IsAssignableFrom(rawValue.GetType()))
        //                    {
        //                        prop.SetValue(control, rawValue);
        //                    }
        //                    else
        //                    {
        //                        throw new InvalidCastException(
        //                            $"Cannot assign delegate of type {rawValue.GetType()} to property {prop.Name} of type {prop.PropertyType}");
        //                    }
        //                }
        //                else if (rawValue != null && !prop.PropertyType.IsAssignableFrom(rawValue.GetType()))
        //                {
        //                    // Try type conversion for non-delegate values
        //                    var converted = Convert.ChangeType(rawValue, prop.PropertyType);
        //                    prop.SetValue(control, converted);
        //                }
        //                else
        //                {
        //                    prop.SetValue(control, rawValue);
        //                }
        //            }
        //        }
        //    }

        //    return control;
        //}
        public static TControl CreateControl<TControl>(Dictionary<string, object> propertyValues = null)
            where TControl : new()
        {
            var control = new TControl();

            if (propertyValues != null)
            {
                var type = typeof(TControl);

                foreach (var kvp in propertyValues)
                {
                    var prop = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        var value = kvp.Value;

                        // Special handling for Func<,> or delegate properties
                        if (value != null && typeof(Delegate).IsAssignableFrom(value.GetType()))
                        {
                            // Ensure delegate type matches property type
                            if (prop.PropertyType.IsAssignableFrom(value.GetType()))
                            {
                                prop.SetValue(control, value);
                            }
                            else
                            {
                                throw new InvalidCastException(
                                    $"Cannot assign delegate of type {value.GetType()} to property {prop.Name} of type {prop.PropertyType}");
                            }
                        }
                        else if (value != null && !prop.PropertyType.IsAssignableFrom(value.GetType()))
                        {
                            // Try type conversion for non-delegate values
                            value = Convert.ChangeType(value, prop.PropertyType);
                            prop.SetValue(control, value);
                        }
                        else
                        {
                            prop.SetValue(control, value);
                        }
                    }
                }
            }

            return control;
        }
    }

}
