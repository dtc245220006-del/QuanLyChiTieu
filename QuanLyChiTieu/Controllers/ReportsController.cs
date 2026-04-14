using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class ReportsController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public ReportsController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // GET: /Reports/Index?year=2026
        public async Task<IActionResult> Index(int? year)
        {
            var y = year ?? DateTime.Now.Year;

            // Aggregate by category for given year
            var byCategory = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.TransactionDate.HasValue && t.TransactionDate.Value.Year == y)
                .GroupBy(t => t.Category != null ? t.Category.CategoryName ?? t.CategoryId : t.CategoryId)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            var categoryLabels = byCategory.Select(x => x.Category).ToArray();
            var categoryData = byCategory.Select(x => x.Total).ToArray();

            // Aggregate by month for given year
            var monthTotals = await _context.Transactions
                .Where(t => t.TransactionDate.HasValue && t.TransactionDate.Value.Year == y)
                .GroupBy(t => t.TransactionDate.Value.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(t => t.Amount) })
                .ToListAsync();

            // Build full 12-month arrays (0 if missing)
            var monthLabels = Enumerable.Range(1, 12).Select(m => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m)).ToArray();
            var monthData = new decimal[12];
            foreach (var m in monthTotals) monthData[m.Month - 1] = m.Total;

            // Pass JSON to view for Chart.js
            ViewBag.Year = y;
            ViewBag.CategoryLabels = JsonSerializer.Serialize(categoryLabels);
            ViewBag.CategoryData = JsonSerializer.Serialize(categoryData);
            ViewBag.MonthLabels = JsonSerializer.Serialize(monthLabels);
            ViewBag.MonthData = JsonSerializer.Serialize(monthData);

            return View();
        }
    }
}