using System;
using System.Collections.Generic;

namespace QuanLyChiTieu.Models;

public partial class UserAccount
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
