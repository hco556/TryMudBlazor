


using Northwind.Odata.Api.Data;
using Shared.Models;

namespace Northwind.OData.Api.Helpers
{
    internal static class ODataAPIDbHelper
    {
        public static void SeedDb(NorthwindContext db)
        {

            db.Orders.Add(new Order { CustomerId = "Customer1", EmployeeId = 1, OrderDate = DateTime.Now, OrderId = 1 });
            db.Orders.Add(new Order { CustomerId = "Customer2", EmployeeId = 1, OrderDate = DateTime.Now, OrderId = 2 });
            db.Orders.Add(new Order { CustomerId = "Customer3", EmployeeId = 2, OrderDate = DateTime.Now, OrderId = 3 });
            db.Orders.Add(new Order { CustomerId = "Customer4", EmployeeId = 2, OrderDate = DateTime.Now, OrderId = 4 });
            db.Orders.Add(new Order { CustomerId = "Customer5", EmployeeId = null, OrderDate = DateTime.Now, OrderId = 5 });

            db.Employees.Add(
                   new Employee
                   {
                       EmployeeId = 1,
                       FirstName = "Jane",
                       LastName = "Doe",
                       Title = "Software Engineer",
                       TitleOfCourtesy = "Mr.",
                       BirthDate = DateTime.Now.AddYears(-25),
                       HireDate = DateTime.Now,
                       Address = "123 Main St",
                       City = "Anytown",
                       Region = "CA",
                       PostalCode = "12345",
                       Country = "USA",
                       HomePhone = "555-1234",
                       Extension = "123",
                       Notes = "New employee",
                       PhotoPath = "/photos/janedoe.jpg"

                   });
            db.Employees.Add(new Employee
            {
                EmployeeId = 2,
                FirstName = "Johnny",
                LastName = "Doe",
                Title = "Software Engineer",
                TitleOfCourtesy = "Mr.",
                BirthDate = DateTime.Now.AddYears(-25),
                HireDate = DateTime.Now,
                Address = "123 Main St",
                City = "Anytown",
                Region = "CA",
                PostalCode = "12345",
                Country = "USA",
                HomePhone = "555-1234",
                Extension = "123",
                Notes = "New employee",
                PhotoPath = "/photos/johnnydoe.jpg"
            });
            db.Employees.Add(new Employee
            {
                EmployeeId = 3,
                FirstName = "Mr",
                LastName = "Manager",
                Title = "Boss",
                TitleOfCourtesy = "Mr.",
                BirthDate = DateTime.Now.AddYears(-45),
                HireDate = DateTime.Now,
                Address = "123 Main St",
                City = "Anytown",
                Region = "CA",
                PostalCode = "12345",
                Country = "USA",
                HomePhone = "555-1234",
                Extension = "123",
                Notes = "Boss",
                PhotoPath = "/photos/boss.jpg"
            });
            //if (!db.Customers.Any())
            //{
            //    db.Add(new Customer
            //    {
            //        CustomerId = 1,
            //        ContactName = "Sue",
            //        Country
            //    });

            //    db.Add(new Customer
            //    {
            //        Id = 2,
            //        Name = "Joe",
            //        CustomerType = CustomerType.Wholesale,
            //        CreditLimit = 5100,
            //        CustomerSince = new DateTime(2022, 12, 12)
            //    });

            db.SaveChanges();
            //}
        }
    }
}
