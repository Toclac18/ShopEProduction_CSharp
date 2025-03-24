using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class PurchaseHistoryDetail
{
    public int Id { get; set; }

    public int HistoryId { get; set; }

    public int ProductDetailId { get; set; }

    public double PriceAtPurchase { get; set; }

    public bool? IsRentedFlg { get; set; }

    public bool? IsBoughtFlg { get; set; }

    public virtual PurchaseHistory History { get; set; } = null!;

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual ICollection<WalletHistoryDetail> WalletHistoryDetails { get; set; } = new List<WalletHistoryDetail>();
}
