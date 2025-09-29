using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Address
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public string? House { get; set; }

    public string? Apartment { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
