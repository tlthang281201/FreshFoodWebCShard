using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ShortDesc { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? CatId { get; set; }

    public int Price { get; set; }

    public int? Discount { get; set; }

    public string Thumb { get; set; } = null!;

    public bool BestSeller { get; set; }

    public bool Active { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateModified { get; set; }

    public string Tags { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public int Stocks { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();
}
