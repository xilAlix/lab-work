using System;
using System.Collections.Generic;

namespace Variant10.Models;

public partial class FacturingCompany
{
    public int firmId { get; set; }

    public string firmName { get; set; } = null!;

    public string adress { get; set; } = null!;

    public string? directorSurname { get; set; }

    public virtual ICollection<FoodProduction> FoodProductions { get; set; } = new List<FoodProduction>();
}
