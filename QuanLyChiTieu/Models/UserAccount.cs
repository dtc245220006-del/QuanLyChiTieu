using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieu.Models;

public partial class UserAccount
{
    public string? UserId { get; set; } = null!;
    [Required]

    public string? Username { get; set; } = null!;
    [Required]

    public string? Password { get; set; } = null!;
    [Required]
    [EmailAddress]

    public string? Email { get; set; } = null!;

    // Role property added to distinguish Admin / User
    public string? Role { get; set; } = "User";

    public virtual ICollection<Budget>? Budgets { get; set; } = new List<Budget>();

    public virtual ICollection<Wallet>? Wallets { get; set; } = new List<Wallet>();
}
