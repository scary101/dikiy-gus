using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class Characteristic
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Unit { get; set; }

    public virtual ICollection<CategoryCharacteristic> CategoryCharacteristics { get; set; } = new List<CategoryCharacteristic>();

    public virtual ICollection<ProductCharacteristic> ProductCharacteristics { get; set; } = new List<ProductCharacteristic>();
}
