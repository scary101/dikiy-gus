using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Payout
{
    public int Id { get; set; }

    public int EntrepreneurId { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Entrepreneur Entrepreneur { get; set; } = null!;
}
