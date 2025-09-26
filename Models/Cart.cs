using System;
using System.Collections.Generic;

namespace Shopping_Web.Models;

public partial class Cart
{
    public string Username { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int VariantId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Account UsernameNavigation { get; set; } = null!;

    public virtual ProductVariant Variant { get; set; } = null!;
}
