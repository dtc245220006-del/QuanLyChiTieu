using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;
using System.Diagnostics;

namespace QuanLyChiTieu.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.TotalThu = _context.Transactions
            .Where(x => x.Category.Type == "Thu")
            .Sum(x => (double?)x.Amount) ?? 0;

            ViewBag.TotalChi = _context.Transactions
                .Where(x => x.Category.Type == "Chi")
                .Sum(x => (double?)x.Amount) ?? 0;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
