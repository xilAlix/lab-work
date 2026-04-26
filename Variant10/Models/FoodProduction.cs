using System;
using System.Collections.Generic;

namespace Variant10.Models;

public partial class FoodProduction
{
    public int firmId { get; set; }

    public int productId { get; set; }

    public float productionVolume { get; set; }

    public virtual FacturingCompany? firmIdNavigation { get; set; }

    public virtual FoodProduct? productIdNavigation { get; set; }
}
