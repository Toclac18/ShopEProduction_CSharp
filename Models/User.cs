using System;
using System.Collections.Generic;

namespace ShopEProduction.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Fullname { get; set; }

    public string? UserImage { get; set; }

    public string Email { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public DateTime? UserCreateAt { get; set; }

    public int? UserPoint { get; set; }

    public int? UserRoleId { get; set; }

    public bool? UserStatus { get; set; }

    public virtual Role? UserRole { get; set; }
}
