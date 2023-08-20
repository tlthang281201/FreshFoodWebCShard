using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Avatar { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public bool Active { get; set; }

    public string Password { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
