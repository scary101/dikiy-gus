using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class SupplyItem
{
    public int Id { get; set; }

    public int SupplyId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supply Supply { get; set; } = null!;
}
