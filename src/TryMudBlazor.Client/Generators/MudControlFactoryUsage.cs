using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MudBlazor;
using MudBlazor.Extensions;

namespace TryMudBlazor.Client.Generators
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using MudBlazor;
    using MudBlazor.Extensions.Components;
    //using MudBlazor.Northwind.ViewModels;
    using System;
    using System;
    using System.Collections.Generic;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection;

    public static class MudControlFactoryUsage
    {

        // ✅ MudCheckBox<bool>
        public static MudCheckBox<bool> CreateCheckBoxFromModel<TModel>(TModel model, string propertyName)
        {
            var property = typeof(TModel).GetProperty(propertyName);
            var attributes = property.GetCustomAttributes(true);

            var dict = new Dictionary<string, object>();

            // Required → MudCheckBox.Required
            if (attributes.OfType<RequiredAttribute>().Any())
                dict["Required"] = true;

            // DisplayName → MudCheckBox.Label
            var displayNameAttr = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayNameAttr != null)
                dict["Label"] = displayNameAttr.DisplayName;

            // ReadOnly → MudCheckBox.Disabled
            var readOnlyAttr = attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
            if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
                dict["Disabled"] = true;

            // Bind to model property
            dict["Checked"] = property.GetValue(model);
            dict["CheckedChanged"] = (Action<bool>)(val => property.SetValue(model, val));
            dict["CheckedExpression"] = () => (bool)property.GetValue(model)!;

            return MudControlFactory.CreateControl<MudCheckBox<bool>>(dict);
        }

        // ✅ MudExSelect<T>
        public static MudExSelect<T> CreateExSelectFromModel<TModel, T>(TModel model, string propertyName, IEnumerable<T> items, bool multiSelection = false)
        {
            var property = typeof(TModel).GetProperty(propertyName);
            var attributes = property.GetCustomAttributes(true);

            var dict = new Dictionary<string, object>();

            // Required → MudExSelect.Required
            if (attributes.OfType<RequiredAttribute>().Any())
                dict["Required"] = true;

            // DisplayName → MudExSelect.Label
            var displayNameAttr = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayNameAttr != null)
                dict["Label"] = displayNameAttr.DisplayName;

            // ReadOnly → MudExSelect.Disabled
            var readOnlyAttr = attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
            if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
                dict["Disabled"] = true;

            // Items
            dict["ItemCollection"] = items;
            dict["MultiSelection"] = multiSelection;

            if (multiSelection)
            {
                dict["SelectedValues"] = property.GetValue(model);
                dict["SelectedValuesChanged"] = (Action<HashSet<T>>)(val => property.SetValue(model, val));
            }
            else
            {
                dict["Value"] = property.GetValue(model);
                dict["ValueChanged"] = (Action<T>)(val => property.SetValue(model, val));
            }

            return MudControlFactory.CreateControl<MudExSelect<T>>(dict);
        }

        // ✅ MudDatePicker
        public static MudDatePicker CreateDatePickerFromModel<TModel>(TModel model, string propertyName)
        {
            var property = typeof(TModel).GetProperty(propertyName);
            var attributes = property.GetCustomAttributes(true);

            var dict = new Dictionary<string, object>();

            // Required → MudDatePicker.Required
            if (attributes.OfType<RequiredAttribute>().Any())
                dict["Required"] = true;

            // DisplayName → MudDatePicker.Label
            var displayNameAttr = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayNameAttr != null)
                dict["Label"] = displayNameAttr.DisplayName;

            // Range → MinDate / MaxDate
            var rangeAttr = attributes.OfType<RangeAttribute>().FirstOrDefault();
            if (rangeAttr != null)
            {
                dict["MinDate"] = Convert.ToDateTime(rangeAttr.Minimum);
                dict["MaxDate"] = Convert.ToDateTime(rangeAttr.Maximum);
            }

            // ReadOnly → MudDatePicker.ReadOnly
            var readOnlyAttr = attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
            if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
                dict["ReadOnly"] = true;

            // Bind to model property
            dict["Date"] = property.GetValue(model);
            dict["DateChanged"] = (Action<DateTime?>)(val => property.SetValue(model, val));
            dict["DateExpression"] = () => (DateTime?)property.GetValue(model);

            return MudControlFactory.CreateControl<MudDatePicker>(dict);
        }
    }

}
