using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Entrepreneur
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? AccountNumber { get; set; }

    public int? WalletId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Payout> Payouts { get; set; } = new List<Payout>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();

    public virtual User User { get; set; } = null!;

    public virtual Wallet? Wallet { get; set; }
}
