using System;
using System.Collections.Generic;

namespace ShoeShop.Models;

public partial class News
{
    public int PostId { get; set; }

    public string Title { get; set; } = null!;

    public string Scontents { get; set; } = null!;

    public string Contents { get; set; } = null!;

    public string Thumb { get; set; } = null!;

    public bool Published { get; set; }

    public string Alias { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public int Views { get; set; }
}
