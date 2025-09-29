using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Supply
{
    public int Id { get; set; }

    public int EntrepreneurId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Entrepreneur Entrepreneur { get; set; } = null!;

    public virtual ICollection<SupplyItem> SupplyItems { get; set; } = new List<SupplyItem>();
}
