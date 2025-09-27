using System;
using System.Collections.Generic;

namespace Shopping_Web.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? UnitPrice { get; set; }

    public int? UnitsInStock { get; set; }

    public string? Image { get; set; }

    public string? ProductStatus { get; set; }

    public int? CategoryId { get; set; }

    public bool? IsFeatured { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
