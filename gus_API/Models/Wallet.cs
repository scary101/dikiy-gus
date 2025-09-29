using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Wallet
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal? Balance { get; set; }

    public virtual ICollection<Accrual> Accruals { get; set; } = new List<Accrual>();

    public virtual ICollection<Entrepreneur> Entrepreneurs { get; set; } = new List<Entrepreneur>();

    public virtual User User { get; set; } = null!;
}
