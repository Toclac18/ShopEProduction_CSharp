using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class WalletHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal CurrentBalance { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WalletHistoryDetail> WalletHistoryDetails { get; set; } = new List<WalletHistoryDetail>();
}
