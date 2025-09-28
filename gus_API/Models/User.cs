using System;
using System.Collections.Generic;

namespace gus_API.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Attempt { get; set; }

    public string? Salt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ConfirmationCode> ConfirmationCodes { get; set; } = new List<ConfirmationCode>();

    public virtual ICollection<Entrepreneur> Entrepreneurs { get; set; } = new List<Entrepreneur>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }

    public virtual Telegram? Telegram { get; set; }

    public virtual ICollection<UserBan> UserBans { get; set; } = new List<UserBan>();

    public virtual Wallet? Wallet { get; set; }
}
