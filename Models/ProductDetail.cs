using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? DetailDesc { get; set; }

    public bool? Status { get; set; }

    public virtual Product Product { get; set; } = null!;
}
