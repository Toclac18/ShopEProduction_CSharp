using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ProductType { get; set; }

    public double Price { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string DetailDesc { get; set; } = null!;

    public string DetailPrivateDesc { get; set; } = null!;

    public DateTime? Duration { get; set; }

    public bool? IsRentedFlg { get; set; }

    public bool? IsBoughtFlg { get; set; }

    public bool? Status { get; set; }

    public virtual Product Product { get; set; } = null!;
}
