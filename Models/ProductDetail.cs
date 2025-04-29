using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ProductType { get; set; }

    public double Price { get; set; }

    public DateTime? ReleasedDate { get; set; }

    public string DetailDesc { get; set; } = null!;

    public string DetailPrivateDesc { get; set; } = null!;

    public bool? IsRentedFlg { get; set; }

    public bool? IsBoughtFlg { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; } = new List<PurchaseHistoryDetail>();

    public virtual ICollection<RentHistoryDetail> RentHistoryDetails { get; set; } = new List<RentHistoryDetail>();

    public virtual ICollection<RentInProcess> RentInProcesses { get; set; } = new List<RentInProcess>();
}
