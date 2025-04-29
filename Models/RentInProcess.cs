using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class RentInProcess
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductDetailId { get; set; }

    public DateTime RentedDate { get; set; }

    public DateTime ExpiredDate { get; set; }

    public bool RentedType { get; set; }

    public int? Duration { get; set; }

    public bool? IsExtended { get; set; }

    public bool? IsExpired { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
