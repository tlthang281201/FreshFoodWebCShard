using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string CatName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Published { get; set; }

    public string? Alias { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
