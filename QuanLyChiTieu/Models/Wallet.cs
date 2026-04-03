using System;
using System.Collections.Generic;

namespace QuanLyChiTieu.Models;

public partial class Wallet
{
    public string WalletId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string WalletName { get; set; } = null!;

    public decimal? Balance { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual UserAccount User { get; set; } = null!;
}
