using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class Discount
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public double DiscountValue { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; } = new List<PurchaseHistoryDetail>();

    public virtual ICollection<RentHistoryDetail> RentHistoryDetails { get; set; } = new List<RentHistoryDetail>();

    public virtual User User { get; set; } = null!;
}
