// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Northwind.Odata.Api.Client;
using Northwind.Odata.Api.Client.Models;
using Northwind.Odata.Api.Client.Odata;
using Northwind.Odata.Api.Client.Odata.Employees;
using Northwind.Odata.Api.Client.Odata.Employees.Item;
using Northwind.Odata.Api.Client.Odata.Orders.Item.Customer;
using Northwind.Odata.Api.Client.Odata.Orders.Item.Employee;
using System;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
// API requires no authentication, so use the anonymous
// authentication provider
var authProvider = new AnonymousAuthenticationProvider();
// Create request adapter using the HttpClient-based implementation
var adapter = new HttpClientRequestAdapter(authProvider);
// Create the API client
var client = new  NorthwindClient(adapter);

try
{
   
  EmployeesRequestBuilder employeesRequestBuilder = client.Odata.Employees;
    var employees = await employeesRequestBuilder.GetAsync();

    Console.WriteLine($"Retrieved {employees.Value?.Count} employees.");
    employeesRequestBuilder.PostAsync(new Employee
    {
        FirstName = "John",
        LastName = "Doe",
        Title = "Software Engineer",
        TitleOfCourtesy = "Mr.",
        BirthDate = DateTimeOffset.Now.AddYears(-30),
        HireDate = DateTimeOffset.Now,
        Address = "123 Main St",
        City = "Anytown",
        Region = "CA",
        PostalCode = "12345",
        Country = "USA",
        HomePhone = "555-1234",
        Extension = "123",
        Notes = "New employee",
        PhotoPath = "/photos/johndoe.jpg"

    });
    employees = await employeesRequestBuilder.GetAsync();

    //var customRequestBuilder = new CustomRequestBuilder($"me/drive/items/{itemId}", graphClient.RequestAdapter);
    //var driveItem = await customRequestBuilder.GetAsync(DriveItem.CreateFromDiscriminatorValue, rc =>
    //{
    //    rc.QueryParameters.Select = ["id", "name"];
    //    rc.QueryParameters.Expand = ["versions"];
    //});
    //https://localhost:5000/odata/Employees?%24top=50
    var employeesFitered = await employeesRequestBuilder.WithUrl("https://localhost:5000/odata/Employees?$top=1").GetAsync();
    var test = await employeesRequestBuilder.GetAsync(rc =>
    {
        rc.QueryParameters.Filter = "lastName eq 'Doe'";
    });
 
    var e = test.Value;
    Console.WriteLine($"Retrieved {employees.Value?.Count} employees.");
    //https://martin-machacek.com/blogPost/fb5a9e4e-c49f-4de7-b32b-46f18f6fb85e.....................................................................
    
//    var graphClient = new GraphServiceClient(credential);
//    You need to first get drive of the user:

//    var drive = graphClient.Me.Drive.GetAsync();
//    Then use the drive id to get the drive item:

//var driveItem = graphClient.Drives[drive.Id].Items[itemId].GetAsync();
//    With the custom request builder, you can call the endpoint directly:

//var customRequestBuilder = new CustomRequestBuilder($"me/drive/items/{itemId}", graphClient.RequestAdapter);
//    var driveItem = await customRequestBuilder.GetAsync(DriveItem.CreateFromDiscriminatorValue, rc =>
//    {
//        rc.QueryParameters.Select = ["id", "name"];
//        rc.QueryParameters.Expand = ["versions"];
//    });
//    Another example

//If you want to get the borders of a workbook range, you will find out that the SDK doesn't generate a code to retrieve or update the workbook range border.

//You can use the custom request builder to call the endpoint directly:

//me / drive / items /{ drive_item_id}/ workbook / worksheets /{ sheet_id}/ range / format / borders
//Code:

//    var path = "me/drive/items/{drive_item_id}/workbook/worksheets/{sheet_id}/range/format/borders";
//    var customRequestBuilder = new CustomRequestBuilder(path, graphClient.RequestAdapter);
//    var result = await customRequestBuilder.GetAsync();
    //var customRequestBuilder = new CustomRequestBuilder($"me/drive/items/{itemId}", graphClient.RequestAdapter);
    //var driveItem = await customRequestBuilder.GetAsync(DriveItem.CreateFromDiscriminatorValue, rc =>
    //{
    //    rc.QueryParameters.Select = ["id", "name"];
    //    rc.QueryParameters.Expand = ["versions"];
    //});

    //// GET /posts/{id}
    //var specificPostId = 5;
    //var specificPost = await client.Posts[specificPostId].GetAsync();
    //Console.WriteLine($"Retrieved post - ID: {specificPost?.Id}, Title: {specificPost?.Title}, Body: {specificPost?.Body}");

    //// POST /posts
    //var newPost = new Post
    //{
    //    UserId = 42,
    //    Title = "Testing Kiota-generated API client",
    //    Body = "Hello world!"
    //};

    //var createdPost = await client.Posts.PostAsync(newPost);
    //Console.WriteLine($"Created new post with ID: {createdPost?.Id}");

    //// PATCH /posts/{id}
    //var update = new Post
    //{
    //    // Only update title
    //    Title = "Updated title"
    //};

    //var updatedPost = await client.Posts[specificPostId].PatchAsync(update);
    //Console.WriteLine($"Updated post - ID: {updatedPost?.Id}, Title: {updatedPost?.Title}, Body: {updatedPost?.Body}");

    //// DELETE /posts/{id}
    //await client.Posts[specificPostId].DeleteAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
