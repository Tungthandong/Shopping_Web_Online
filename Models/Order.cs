using System;
using System.Collections.Generic;

namespace Shopping_Web.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string Username { get; set; } = null!;

    public int TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string? OrderStatus { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public decimal ShipCost { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Account UsernameNavigation { get; set; } = null!;
}
