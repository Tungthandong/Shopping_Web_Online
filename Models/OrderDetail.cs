using System;
using System.Collections.Generic;

namespace Shopping_Web.Models;

public partial class OrderDetail
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public int? VariantId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
