using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class ProductCharacteristic
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int CharacteristicId { get; set; }

    public string? Value { get; set; }

    public virtual Characteristic Characteristic { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
