
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace MudBlazor.Examples.Data.ViewModels;
//Start-Template//
public class EmployeeViewModel
{
    [DisplayName("Employee Id")]
    [ReadOnly(true)]
    public int EmployeeId { get; set; }


    [Range(0, 999999.99)]
    public decimal? Salary { get; set; }

    public DateTime? HireDate { get; set; }

    public bool IsActive { get; set; }

    [StringLength(20, MinimumLength = 3)]
    [DisplayName("Date Created")]
    [ReadOnly(true)]
    public DateTime CreationDate { get; set; } = DateTime.Now;

    [DisplayName("Last Name")]
    [Required(ErrorMessage ="Last Name is required")]
    public string LastName { get; set; } = null!;
    [DisplayName("First Name")]
    [Required(ErrorMessage ="First Name is required")]
    public string FirstName { get; set; } = null!;
    [DisplayName("Title")]
    public string? Title { get; set; }

    [DisplayName("PassWord")]
    [Required(ErrorMessage ="Password is Required")] // optional: enforce non-empty
    public string Password { get; set; } = null!;

    [DisplayName("Title Of Courtesy")]
    public string? TitleOfCourtesy { get; set; }

    [DisplayName("Birth Date")]
    public DateTime? BirthDate { get; set; }


    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? HomePhone { get; set; }

    public string? Extension { get; set; }

    public byte[]? Photo { get; set; }

    public string? Notes { get; set; }

    //public int? ReportsTo { get; set; }

    public string? PhotoPath { get; set; }


    [DisplayName("Orders for Employee")]
    public virtual List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

    [DisplayName("Reports To")]
    public virtual EmployeeViewModel? ReportsTo { get; set; }

   // public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();

    public override string ToString()
    {
        return  FirstName + " " + LastName;
    }
}
//End-Template//
