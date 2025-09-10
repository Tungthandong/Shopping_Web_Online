using System;
using System.Collections.Generic;

namespace Shopping_Web.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public string? Birthdate { get; set; }

    public string? Gender { get; set; }

    public string? Role { get; set; }

    public string? AccountStatus { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
