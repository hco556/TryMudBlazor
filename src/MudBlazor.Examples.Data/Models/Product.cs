        using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MudBlazor.Examples.Data.Models
{
    public partial class Product
    {
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }

        [Column("ProductUniqueID")]
        public Guid? ProductUniqueId { get; set; }

        [Required]
        [MaxLength(40)]
        public string ProductName { get; set; } = null!;

        [Column("SupplierID")]
        public int? SupplierId { get; set; }

        [Column("CategoryID")]
        public int? CategoryId { get; set; }

        [MaxLength(20)]
        public string? QuantityPerUnit { get; set; }

        [Column(TypeName = "money")]
        [Range(0, 999999.99)]
        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        [Column("ReferenceUniqueID")]
        public Guid ReferenceUniqueId { get; set; }

        [MaxLength(500)]
        public string? ProductUri { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual Supplier? Supplier { get; set; }
    }
}
