//using MudBlazor.Northwind.Services;
using Northwind.CodeGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TryMudBlazor.Client.Generators
{
    /// <summary>
    /// Generates a MudBlazor EditForm razor component source for a given ViewModel type.
    /// - Uses DataAnnotations validation (<see cref="DataAnnotationsValidator"/>).
    /// - Reads a FormControl attribute (by name) and emits controls:
    ///   - Text / Password / Numeric -> MudTextField / MudNumericField
    ///   - Datetime -> MudDatePicker
    ///   - DropDownSingleSelect -> MudExSelect with MultiSelection="false"
    ///   - DropDownMultiSelect  -> MudExSelect with MultiSelection="true"
    /// </summary>
    public static class MudFormGenerator
    {

        public static string GetFriendlyTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                var generic = t.GetGenericTypeDefinition();
                var argNames = string.Join(", ", t.GetGenericArguments().Select(GetFriendlyTypeName));
                return $"{generic.Name.Split('`')[0]}<{argNames}>";
            }
            if (t.IsArray)
                return $"{GetFriendlyTypeName(t.GetElementType() ?? t)}[]";
            return t.Name;
        }

        public static List<string> FormatAttributes(object[] attrs)
        {
            if (attrs == null || attrs.Length == 0)
                return new List<string>();

            var parts = new List<string>();
            foreach (var a in attrs)
            {
                switch (a)
                {
                    case DisplayNameAttribute da:
                        parts.Add($"DisplayName=\"{da.DisplayName}\"");
                        break;
                    case DisplayAttribute d:
                        var display = $"Display(Name=\"{d.Name}\")";
                        parts.Add(display);
                        break;
                    case RequiredAttribute _:
                        parts.Add("Required");
                        break;
                    case KeyAttribute _:
                        parts.Add("Key");
                        break;
                    case ReadOnlyAttribute ro:
                        parts.Add($"ReadOnly={ro.IsReadOnly}");
                        break;
                    default:
                        // Fallback to attribute type name (without "Attribute" suffix)
                        var name = a.GetType().Name;
                        if (name.EndsWith("Attribute"))
                            name = name.Substring(0, name.Length - 9);
                        parts.Add(name);
                        break;
                }
            }
            return parts;
        }
        public static string GenerateViewModel(Type itemType, List<string> selectedProperties = null)
        { 
        
            //Employee employeeTemplate = new Employee();
            //item = default;
            var item = Activator.CreateInstance(itemType);
            Type templateType = item.GetType();
            Console.WriteLine(templateType.Name);
            MemberInfo[] members = templateType.GetMembers();
            var vmClassName =  item.GetType().ToString() + "ViewModel";
            StringBuilder viewModelStringBuilder = new StringBuilder();
            viewModelStringBuilder.AppendLine("public class " + vmClassName + "{");
            foreach (var member in members)
            {
                if (selectedProperties != null)
                {
                    if (!selectedProperties.Contains(member.Name))
                        continue;
                }
                var memberType = member.MemberType;

                if (memberType.ToString() == "Property")
                {

                    PropertyInfo propInfo = templateType.GetProperty(member.Name);
                    Type propType = propInfo.PropertyType;
                    var shortPropTypeName = propType.Name;
                    string label = Regex.Replace(member.Name, "(?<!^)([A-Z])", " $1");
                    viewModelStringBuilder.AppendLine("     [DisplayName(\"" + label + "\")]");
                    //if (member.Name == "Title" || member.Name == "LastName")
                    //    break;
                    if (propType.Name.Contains("Nullable") || propType.Name.Contains("ICollection"))
                    {
                        //System.Nullable`1[[System.Int32,
                        var startIdx = propType.FullName.IndexOf("[[") + 2;
                        // var propTypeName = propType.FullName.Replace("System.Nullable`1[[", "");
                        var endIdx = propType.FullName.IndexOf(",");
                        shortPropTypeName = propType.FullName.Substring(startIdx, endIdx - startIdx);

                    }
                    if (propType.Name.Contains("Nullable") || propInfo.IsNullable())
                    {
                        shortPropTypeName += "?";
                    }
                    if (propType.Name.Contains("ICollection"))
                    {
                        var collectionElementType = ReflectionExtensions.GetCollectionElementType(templateType, member.Name);
                        shortPropTypeName = "ICollection<" + collectionElementType + ">";

                    }
                    var attrs = propInfo.GetCustomAttributes(true).Cast<object>().ToArray();
                    //                       TypeName = GetFriendlyTypeName(p.PropertyType),
                    var attributes = FormatAttributes(attrs);
                    foreach (var attrib in attributes)
                    {
                        viewModelStringBuilder.AppendLine("     [" + attrib + "]");
                    }
                        // Console.WriteLine(shortPropTypeName + " " + member.Name);

                     viewModelStringBuilder.AppendLine("     public " + shortPropTypeName + " " + member.Name + "{ get; set; }");
                }

            }
            viewModelStringBuilder.AppendLine("}");
            return viewModelStringBuilder.ToString();
        }
        public static string GenerateMudForm(Type viewModelType,
            string componentNamespace = "MudBlazor.Northwind.Components.Pages.Employee",
            string componentName = null)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
            componentName ??= viewModelType.Name + "Form";

            var vmNamespace = viewModelType.Namespace ?? "MudBlazor.Northwind.ViewModels";
            var props = viewModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0)
                .ToList();

            var sb = new StringBuilder();

            // Header / usings / route
            sb.AppendLine($"@page \"/{componentName.ToLowerInvariant()}\"");
            sb.AppendLine($"@using Microsoft.AspNetCore.Components");
             sb.AppendLine($"@using Microsoft.AspNetCore.Components.Forms");
             sb.AppendLine($"@using MudBlazor");
             sb.AppendLine($"@inject KiotaRequestBuilder kiotaRequestBuilder");
             sb.AppendLine($"@using MudBlazor.Northwind.Mappers.Employee");
             sb.AppendLine($"@using MudBlazor.Northwind.Services");
             sb.AppendLine($"@using MudBlazor.Extensions");
             sb.AppendLine($"@using MudBlazor.Extensions.Options");
             sb.AppendLine($"@inject NavigationManager NavigationManager");
            sb.AppendLine($"@using {vmNamespace}");
            sb.AppendLine();
            sb.AppendLine($"@* Auto-generated MudBlazor form for {viewModelType.FullName} *@");
            sb.AppendLine();

            // Begin form
            sb.AppendLine($"<EditForm Model=\"@Model\" OnValidSubmit=\"HandleValidSubmit\">");
            sb.AppendLine($"    <DataAnnotationsValidator />");
            sb.AppendLine($"    <ValidationSummary />");
            sb.AppendLine();
            sb.AppendLine($"    <MudCard Class=\"pa-4\">");
            sb.AppendLine($"        <MudCardContent>");
            sb.AppendLine($"            <MudGrid>");

            // collect select fields to declare
            var selectFields = new List<(PropertyInfo Prop, Type ElementType, bool Multi, string CustomQuery)>();

            foreach (var p in props)
            {
                // Skip indexers / non-public handled earlier; skip navigations if desired by attribute detection
                // Read FormControlAttribute by name to avoid dependency on generator assembly types
                var fcAttr = p.GetCustomAttributes(inherit: false)
                              .FirstOrDefault(a => a.GetType().Name == "FormControlAttribute");

                // By default treat as Text
                string fcType = "Text";
                string customQuery = string.Empty;
                if (fcAttr != null)
                {
                    var tprop = fcAttr.GetType().GetProperty("Type");
                    if (tprop != null && tprop.GetValue(fcAttr) != null)
                        fcType = tprop.GetValue(fcAttr)!.ToString() ?? fcType;

                    var cqProp = fcAttr.GetType().GetProperty("CustomQuery");
                    if (cqProp != null)
                        customQuery = cqProp.GetValue(fcAttr) as string ?? string.Empty;
                }

                // label: DisplayNameAttribute or fallback to split pascal case
                var displayAttr = p.GetCustomAttribute<DisplayNameAttribute>();
                var display = displayAttr?.DisplayName ?? SplitPascalCase(p.Name);

                sb.AppendLine($"                <MudItem xs=\"12\" sm=\"6\">");

                switch (fcType)
                {
                    case "Datetime":
                    case "DateTime":
                        sb.AppendLine($"                    <MudDatePicker Label=\"{Escape(display)}\"");
                        sb.AppendLine($"                                   @bind-Date=\"Model.{p.Name}\"");
                        sb.AppendLine($"                                   PickerVariant=\"PickerVariant.Dialog\" Clearable=\"true\" />");
                        sb.AppendLine($"                    <ValidationMessage For=\"@(() => Model.{p.Name})\" />");
                        break;

                    case "DropDownSingleSelect":
                        {
                            // property type is assumed to be the selected item type
                            var itemType = p.PropertyType;
                            var itemTypeName = GetTypeName(itemType);
                            var itemsField = $"_{p.Name}Items";
                            sb.AppendLine($"                    <MudExSelect T=\"{itemTypeName}\" Label=\"{Escape(display)}\"");
                            sb.AppendLine($"                                 ItemCollection=\"@{itemsField}\"");
                            sb.AppendLine($"                                 Value=\"@Model.{p.Name}\"");
                            sb.AppendLine($"                                 ValueChanged=\"@(({itemTypeName} v) => Model.{p.Name} = v)\"");
                            sb.AppendLine($"                                 MultiSelection=\"false\"");
                            sb.AppendLine($"                                 SearchBox=\"true\"");
                            sb.AppendLine($"                                 Color=\"Color.Primary\">");
                            sb.AppendLine($"                    </MudExSelect>");
                            sb.AppendLine($"                    <ValidationMessage For=\"@(() => Model.{p.Name})\" />");

                            var elementType = itemType;
                            selectFields.Add((p, elementType, false, customQuery));
                        }
                        break;

                    case "DropDownMultiSelect":
                        {
                            // property expected to be a collection: List<T> or similar
                            var elementType = GetCollectionElementType(p.PropertyType) ?? typeof(object);
                            var elementTypeName = GetTypeName(elementType);
                            var itemsField = $"_{p.Name}Items";
                            sb.AppendLine($"                    <MudExSelect T=\"{elementTypeName}\" Label=\"{Escape(display)}\"");
                            sb.AppendLine($"                                 ItemCollection=\"@{itemsField}\"");
                            sb.AppendLine($"                                 SelectedValues=\"@Model.{p.Name}\"");
                            sb.AppendLine($"                                 MultiSelection=\"true\"");
                            sb.AppendLine($"                                 SelectAll=\"true\"");
                            sb.AppendLine($"                                 SearchBox=\"true\"");
                            sb.AppendLine($"                                 Color=\"Color.Primary\">");
                            sb.AppendLine($"                    </MudExSelect>");
                            sb.AppendLine($"                    <ValidationMessage For=\"@(() => Model.{p.Name})\" />");

                            selectFields.Add((p, elementType, true, customQuery));
                        }
                        break;

                    case "Numeric":
                        {
                            var numericType = p.PropertyType == typeof(int) ? "int" : GetTypeName(p.PropertyType);
                            sb.AppendLine($"                    <MudNumericField T=\"{numericType}\" Label=\"{Escape(display)}\" @bind-Value=\"Model.{p.Name}\" />");
                            sb.AppendLine($"                    <ValidationMessage For=\"@(() => Model.{p.Name})\" />");
                        }
                        break;

                    case "Password":
                        sb.AppendLine($"                    <MudTextField Label=\"{Escape(display)}\" @bind-Value=\"Model.{p.Name}\" InputType=\"InputType.Password\" />");
                        sb.AppendLine($"                    <ValidationMessage For=\"@(() => Model.{p.Name})\" />");
                        break;

                    default:
                        // Text
                        sb.AppendLine($"                    <MudTextField Label=\"{Escape(display)}\" @bind-Value=\"Model.{p.Name}\" />");
                        sb.AppendLine($"                    <ValidationMessage For=\"@(() => Model.{p.Name})\" />");
                        break;
                }

                sb.AppendLine($"                </MudItem>");
            }

            sb.AppendLine($"            </MudGrid>");
            sb.AppendLine($"        </MudCardContent>");
            sb.AppendLine();
            sb.AppendLine($"        <MudCardActions Class=\"justify-end\">");
            sb.AppendLine($"            <MudButton Variant=\"Variant.Outlined\" Color=\"Color.Secondary\" OnClick=\"Cancel\">Cancel</MudButton>");
            sb.AppendLine($"            <MudButton Variant=\"Variant.Filled\" Color=\"Color.Primary\" ButtonType=\"ButtonType.Submit\">Save</MudButton>");
            sb.AppendLine($"        </MudCardActions>");
            sb.AppendLine($"    </MudCard>");
            sb.AppendLine();
            sb.AppendLine($"</EditForm>");
            sb.AppendLine();

            // @code section
            sb.AppendLine($"@code {{");
            sb.AppendLine($"    [Parameter] public {viewModelType.FullName} Model {{ get; set; }} = new {viewModelType.FullName}();");
            sb.AppendLine();

            // declare collections for selects
            foreach (var sf in selectFields)
            {
                var fieldName = $"_{sf.Prop.Name}Items";
                var itemTypeName = GetTypeName(sf.ElementType);
                sb.AppendLine($"    private List<{itemTypeName}> {fieldName} = new List<{itemTypeName}>();");
            }
            sb.AppendLine();

            // OnInitializedAsync TODO to populate collections
            sb.AppendLine($"    protected override async Task OnInitializedAsync()");
            sb.AppendLine($"    {{");
            if (selectFields.Count == 0)
            {
                sb.AppendLine($"        await Task.CompletedTask;");
            }
            else
            {
                foreach (var sf in selectFields)
                {
                    var fieldName = $"_{sf.Prop.Name}Items";
                    var itemTypeName = GetTypeName(sf.ElementType);
                    if (!string.IsNullOrWhiteSpace(sf.CustomQuery))
                    {
                        sb.AppendLine($"        // Example: populate {fieldName} using custom query \"{Escape(sf.CustomQuery)}\"");
                        sb.AppendLine($"        // {fieldName} = await KiotaRequestBuilder.SomeMethodThatReturns{itemTypeName}List(\"{Escape(sf.CustomQuery)}\");");
                    }
                    else
                    {
                        sb.AppendLine($"        // TODO: populate {fieldName} (List<{itemTypeName}>) from API or service");
                        sb.AppendLine($"        // e.g. {fieldName} = await KiotaRequestBuilder.Get{itemTypeName}List(...);");
                    }
                }
            }
            sb.AppendLine($"    }}");
            sb.AppendLine();
            sb.AppendLine($"    private async Task HandleValidSubmit()");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        // TODO: handle save, Model passes DataAnnotations validation");
            sb.AppendLine($"        await Task.CompletedTask;");
            sb.AppendLine($"    }}");
            sb.AppendLine();
            sb.AppendLine($"    private void Cancel() => NavigationManager.NavigateTo(\"/\");");
            sb.AppendLine($"}}");

            return sb.ToString();
        }

        private static string GetTypeName(Type t)
        {
            if (t == null) return "object";
            if (t.IsGenericType)
            {
                var def = t.GetGenericTypeDefinition();
                var name = def.Name;
                var backtick = name.IndexOf('`');
                if (backtick > 0) name = name.Substring(0, backtick);
                var args = string.Join(", ", t.GetGenericArguments().Select(GetTypeName));
                return $"{name}<{args}>";
            }
            return t.FullName ?? t.Name;
        }

        private static Type GetCollectionElementType(Type t)
        {
            if (t.IsArray) return t.GetElementType();
            if (t.IsGenericType)
            {
                var args = t.GetGenericArguments();
                if (args.Length == 1) return args[0];
            }

            var ienum = t.GetInterfaces()
                         .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return ienum?.GetGenericArguments().FirstOrDefault();
        }

        private static string Escape(string s)
        {
            if (s == null) return string.Empty;
            return s.Replace("\"", "\\\"");
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
