using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Accrual
{
    public int Id { get; set; }

    public int WalletId { get; set; }

    public int? OrderId { get; set; }

    public decimal Amount { get; set; }

    public decimal? Commission { get; set; }

    public decimal? Payout { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
