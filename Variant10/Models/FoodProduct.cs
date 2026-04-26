using System;
using System.Collections.Generic;

namespace Variant10.Models;

public partial class FoodProduct
{
    public int id { get; set; }

    public string title { get; set; } = null!;

    public string productGroup { get; set; } = null!;

    public string? packageType { get; set; }

    public virtual ICollection<FoodProduction> FoodProductions { get; set; } = new List<FoodProduction>();
}
