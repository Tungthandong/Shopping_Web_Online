using System;
using System.Collections.Generic;

namespace Shopping_Web.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? Quantity { get; set; }

    public string? Note { get; set; }

    public string? FilePath { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
