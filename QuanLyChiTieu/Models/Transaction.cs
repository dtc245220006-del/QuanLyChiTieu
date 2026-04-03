using System;
using System.Collections.Generic;

namespace QuanLyChiTieu.Models;

public partial class Transaction
{
    public string TransactionId { get; set; } = null!;

    public string WalletId { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Wallet Wallet { get; set; } = null!;
}
