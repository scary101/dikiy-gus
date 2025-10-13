using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Supply
{
    public int Id { get; set; }

    public int EntrepreneurId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ManagerId { get; set; }

    public int StatusId { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Entrepreneur Entrepreneur { get; set; } = null!;

    public virtual User? Manager { get; set; }

    public virtual SupplyStatus Status { get; set; } = null!;

    public virtual ICollection<SupplyItem> SupplyItems { get; set; } = new List<SupplyItem>();
}
