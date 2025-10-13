using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class SupplyStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
