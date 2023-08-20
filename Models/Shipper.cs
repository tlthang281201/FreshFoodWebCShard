using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class Shipper
{
    public int ShipperId { get; set; }

    public string ShipperName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Company { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
