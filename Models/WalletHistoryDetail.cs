using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class WalletHistoryDetail
{
    public int Id { get; set; }

    public int HistoryId { get; set; }

    public string HistoryType { get; set; } = null!;

    public DateTime TimeExecution { get; set; }

    public decimal PreValue { get; set; }

    public decimal ChangeAmount { get; set; }

    public decimal PostValue { get; set; }

    public string? Description { get; set; }

    public int? PurchaseDetailId { get; set; }

    public int? RentDetailId { get; set; }

    public virtual WalletHistory History { get; set; } = null!;

    public virtual PurchaseHistoryDetail? PurchaseDetail { get; set; }

    public virtual RentHistoryDetail? RentDetail { get; set; }
}
