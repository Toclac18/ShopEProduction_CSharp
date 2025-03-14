using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ProductCategory { get; set; }

    public int? SoldNumber { get; set; }

    public bool? Status { get; set; }

    public virtual Category ProductCategoryNavigation { get; set; } = null!;

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
