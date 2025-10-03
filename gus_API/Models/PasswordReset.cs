using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class PasswordReset
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; }

    public virtual User User { get; set; } = null!;
}
