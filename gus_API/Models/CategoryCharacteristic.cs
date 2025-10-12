using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class CategoryCharacteristic
{
    public int Id { get; set; }

    public int Categoryid { get; set; }

    public int Characteristicid { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Characteristic Characteristic { get; set; } = null!;
}
