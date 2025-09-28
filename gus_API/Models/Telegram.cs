using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Telegram
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public long TgId { get; set; }

    public DateTime? LinkedAt { get; set; }

    public virtual User? User { get; set; }
}
