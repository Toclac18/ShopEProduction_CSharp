using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int ProductDetailId { get; set; }

    public string ProductDetailName { get; set; } = null!;

    public decimal ProductDetailPrice { get; set; }

    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    internal bool Any()
    {
        throw new NotImplementedException();
    }
}
