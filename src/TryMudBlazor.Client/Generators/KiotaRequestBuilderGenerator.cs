using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TryMudBlazor.Client.Generators
{
    /// <summary>
    /// Generates a KiotaRequestBuilder-style service class source that calls _client.Odata.<Resource>
    /// and (optionally) maps the returned OData client models to view models via provided mapper calls.
    /// 
    /// Usage:
    ///   var queries = new[]
    ///   {
    ///     new QueryDefinition(
    ///         MethodName: "GetEmployeesExcludingId",
    ///         ResourceProperty: "Employees",
    ///         FilterTemplate: "EmployeeId ne {Id}",
    ///         ReturnViewModelType: "MudBlazor.Northwind.ViewModels.EmployeeViewModel",
    ///         SingleResult: false,
    ///         MapperTypeFullName: "MudBlazor.Northwind.Mappers.Employee.EmployeeMapper",
    ///         MapperSingleMethod: "MapEmployeeToViewModel",
    ///         MapperListMethod: "MapEmployeesToViewModels"
    ///     ),
    ///     new QueryDefinition(...),
    ///   };
    ///
    ///   var src = KiotaRequestBuilderGenerator.Generate(
    ///       clientTypeFullName: "Northwind.Odata.Api.Client.NorthwindClient",
    ///       @namespace: "MudBlazor.Northwind.Services.Generated",
    ///       className: "GeneratedKiotaRequestBuilder",
    ///       queries: queries);
    /// </summary>
    public static class KiotaRequestBuilderGenerator
    {
        public sealed record QueryDefinition(
            string MethodName,
            string ResourceProperty,
            string FilterTemplate,
            string ReturnViewModelType,              // full type name for return view model
            bool SingleResult = false,
            string MapperTypeFullName = null,      // e.g. "MudBlazor.Northwind.Mappers.Employee.EmployeeMapper"
            string MapperSingleMethod = null,      // e.g. "MapEmployeeToViewModel"
            string MapperListMethod = null         // e.g. "MapEmployeesToViewModels"
        );

        /// <summary>
        /// Generate the KiotaRequestBuilder source file as a string.
        /// The generated class will have a constructor taking the specified Kiota client type.
        /// Each QueryDefinition produces one async method which accepts a Dictionary<string, object?> parameters
        /// used to replace tokens like {Id} inside FilterTemplate.
        /// </summary>
        public static string Generate(
            string clientTypeFullName,
            IEnumerable<QueryDefinition> queries,
            string @namespace = "MudBlazor.Northwind.Services.Generated",
            string className = "GeneratedKiotaRequestBuilder")
        {
            if (string.IsNullOrWhiteSpace(clientTypeFullName)) throw new ArgumentNullException(nameof(clientTypeFullName));
            var qlist = queries?.ToList() ?? new List<QueryDefinition>();

            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text.RegularExpressions;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();

            sb.AppendLine($"namespace {@namespace};");
            sb.AppendLine();
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly {clientTypeFullName} _client;");
            sb.AppendLine();
            sb.AppendLine($"    public {className}({clientTypeFullName} client)");
            sb.AppendLine("    {");
            sb.AppendLine("        _client = client ?? throw new ArgumentNullException(nameof(client));");
            sb.AppendLine("    }");
            sb.AppendLine();

            // helper: buildFilter method
            sb.AppendLine("    private static string BuildFilter(string template, IDictionary<string, object?>? parameters)");
            sb.AppendLine("    {");
            sb.AppendLine("        if (string.IsNullOrEmpty(template)) return string.Empty;");
            sb.AppendLine("        if (parameters == null || parameters.Count == 0) return template;");
            sb.AppendLine("        string result = template;");
            sb.AppendLine("        foreach (var kv in parameters)");
            sb.AppendLine("        {");
            sb.AppendLine("            var placeholder = \"{\" + kv.Key + \"}\";");
            sb.AppendLine("            var value = kv.Value;");
            sb.AppendLine("            string replacement;");
            sb.AppendLine("            if (value == null) replacement = \"null\";");
            sb.AppendLine("            else if (value is string s) replacement = \"'\" + s.Replace(\"'\", \"''\") + \"'\";"); // wrap strings in single quotes
            sb.AppendLine("            else if (value is DateTime dt) replacement = dt.ToString(\"o\");");
            sb.AppendLine("            else replacement = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;");
            sb.AppendLine("            result = result.Replace(placeholder, replacement);");
            sb.AppendLine("        }");
            sb.AppendLine("        return result;");
            sb.AppendLine("    }");
            sb.AppendLine();

            // generate methods
            foreach (var q in qlist)
            {
                var returnVmFull = q.ReturnViewModelType ?? "object";
                var returnVmSimple = SimpleTypeName(returnVmFull);
                if (q.SingleResult)
                {
                    sb.AppendLine($"    public async Task<{returnVmFull}?> {q.MethodName}(IDictionary<string, object?>? parameters = null)");
                    sb.AppendLine("    {");
                    sb.AppendLine($"        var builder = _client.Odata.{q.ResourceProperty};");
                    sb.AppendLine("        var filter = BuildFilter(@\"" + EscapeString(q.FilterTemplate) + "\", parameters);");
                    sb.AppendLine("        var response = await builder.GetAsync(rc =>");
                    sb.AppendLine("        {");
                    sb.AppendLine("            rc.QueryParameters.Filter = filter;");
                    sb.AppendLine("        });");
                    sb.AppendLine("        var item = response?.Value?.FirstOrDefault();");
                    if (!string.IsNullOrEmpty(q.MapperTypeFullName) && !string.IsNullOrEmpty(q.MapperSingleMethod))
                    {
                        // call mapper
                        sb.AppendLine("        if (item == null) return null;");
                        sb.AppendLine($"        return {q.MapperTypeFullName}.{q.MapperSingleMethod}(item);");
                    }
                    else
                    {
                        sb.AppendLine("        return (item == null) ? default : (object?)item as " + returnVmFull + ";");
                    }
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine($"    public async Task<List<{returnVmFull}>> {q.MethodName}(IDictionary<string, object?>? parameters = null)");
                    sb.AppendLine("    {");
                    sb.AppendLine($"        var builder = _client.Odata.{q.ResourceProperty};");
                    sb.AppendLine("        var filter = BuildFilter(@\"" + EscapeString(q.FilterTemplate) + "\", parameters);");
                    sb.AppendLine("        var response = await builder.GetAsync(rc =>");
                    sb.AppendLine("        {");
                    sb.AppendLine("            rc.QueryParameters.Filter = filter;");
                    sb.AppendLine("        });");
                    sb.AppendLine("        if (response?.Value == null) return new List<" + returnVmFull + ">();");
                    if (!string.IsNullOrEmpty(q.MapperTypeFullName) && !string.IsNullOrEmpty(q.MapperListMethod))
                    {
                        sb.AppendLine($"        var mapped = {q.MapperTypeFullName}.{q.MapperListMethod}(response.Value);");
                        sb.AppendLine($"        return mapped ?? new List<{returnVmFull}>();");
                    }
                    else
                    {
                        // attempt naive mapping: cast each to return type if possible (best-effort)
                        sb.AppendLine($"        var list = new List<{returnVmFull}>();");
                        sb.AppendLine("        foreach (var it in response.Value) { if (it is " + returnVmFull + " v) list.Add(v); }");
                        sb.AppendLine("        return list;");
                    }
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("}"); // end class

            return sb.ToString();
        }

        private static string SimpleTypeName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return fullName;
            var last = fullName.LastIndexOf('.');
            return last >= 0 ? fullName.Substring(last + 1) : fullName;
        }

        private static string EscapeString(string s)
        {
            return s?.Replace("\"", "\"\"") ?? string.Empty;
        }
    }
}