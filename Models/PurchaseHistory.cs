using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class PurchaseHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CartId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ICollection<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; } = new List<PurchaseHistoryDetail>();

    public virtual User User { get; set; } = null!;
}
