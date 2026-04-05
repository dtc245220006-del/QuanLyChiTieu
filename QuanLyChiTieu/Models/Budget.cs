using System;
using System.Collections.Generic;

namespace QuanLyChiTieu.Models;

public partial class Budget
{
    public string? BudgetId { get; set; } = null!;

    public string? UserId { get; set; } = null!;

    public string? CategoryId { get; set; } = null!;

    public decimal LimitAmount { get; set; }

    public string? MonthYear { get; set; } = null!;

    public virtual Category? Category { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
