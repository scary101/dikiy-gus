using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Wallet
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal? Balance { get; set; }

    public virtual ICollection<Accrual> Accruals { get; set; } = new List<Accrual>();

    public virtual Entrepreneur? Entrepreneur { get; set; }

    public virtual User User { get; set; } = null!;
}
