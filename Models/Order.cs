using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? ShipDate { get; set; }

    public string Status { get; set; } = null!;

    public bool Deleted { get; set; }

    public bool Paid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? PaymentId { get; set; }

    public string? Note { get; set; }

    public int? ShipperId { get; set; }

    public int TotalMoney { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();

    public virtual Shipper? Shipper { get; set; }
}
