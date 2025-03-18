using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? SoldNumber { get; set; }

    public int CurrentAvailable { get; set; }

    public int Type { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
