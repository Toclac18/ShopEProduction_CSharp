using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class RentHistoryDetail
{
    public int Id { get; set; }

    public int HistoryId { get; set; }

    public int ProductDetailId { get; set; }

    public DateTime Duration { get; set; }

    public DateTime RentedDate { get; set; }

    public DateTime ExpiredDate { get; set; }

    public double Price { get; set; }

    public int? DiscountId { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual RentHistory History { get; set; } = null!;

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual ICollection<WalletHistoryDetail> WalletHistoryDetails { get; set; } = new List<WalletHistoryDetail>();
}
