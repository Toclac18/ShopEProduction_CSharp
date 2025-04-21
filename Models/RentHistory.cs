using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class RentHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<RentHistoryDetail> RentHistoryDetails { get; set; } = new List<RentHistoryDetail>();

    public virtual User User { get; set; } = null!;
}
