namespace MudBlazor.Examples.Data.ViewModels;
//Start-Template//
public class OrderViewModel
{
    public int OrderId { get; set; }

    public string? CustomerId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? OrderDate { get; set; }

    //public DateTime? RequiredDate { get; set; }

    //public DateTime? ShippedDate { get; set; }

    //public int? ShipVia { get; set; }

    //public decimal? Freight { get; set; }

    //public string? ShipName { get; set; }

    //public string? ShipAddress { get; set; }

    //public string? ShipCity { get; set; }

    //public string? ShipRegion { get; set; }

    //public string? ShipPostalCode { get; set; }

    //public string? ShipCountry { get; set; }

    //public virtual Customer? Customer { get; set; }

    //public virtual EmployeeViewModel? Employee { get; set; }

    // public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    // public virtual Shipper? ShipViaNavigation { get; set; }
    public override string ToString()
    {
        return OrderId + ": Customer " + CustomerId + " - Order Date: " + OrderDate?.ToString("dd/MM/yyyy");
    }
}
//End-Template//
