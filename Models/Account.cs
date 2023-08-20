using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public bool Active { get; set; }

    public string FullName { get; set; } = null!;

    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }
}
