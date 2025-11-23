// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

using Shared.Models;

namespace Northwind.Odata.Api.Models
{
    public static class ApplicationEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();

            modelBuilder.Namespace = "NorthwindService";

            //modelBuilder.EntitySet<Product>("Products"); // Example entity
            //modelBuilder.EntitySet<Category>("Categories"); // Example entity
            //modelBuilder.EntitySet<Order>("Orders"); // Example entity
            modelBuilder.EntitySet<Employee>("Employees"); // Example entity
            // Send as Lower Camel Case Properties, so the JSON looks better:
            modelBuilder.EnableLowerCamelCase();

            return modelBuilder.GetEdmModel();
        }

    }
}
