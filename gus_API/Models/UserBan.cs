using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class UserBan
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Reason { get; set; }

    public virtual User User { get; set; } = null!;
}
