using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public BudgetsController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            // Include bảng Category và User để lấy tên hiển thị
            var budgets = _context.Budgets.Include(b => b.Category).Include(b => b.User);
            var budgetList = await budgets.ToListAsync();
            return View(budgetList);
        }

        // GET: Budgets/Create
        public IActionResult Create()
        {
            // SỬA TẠI ĐÂY: Hiển thị CategoryName và UserName thay vì ID
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "FullName"); // Hoặc UserName tùy model của m
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetId,UserId,CategoryId,LimitAmount,MonthYear")] Budget budget)
        {
            // 1. Tự sinh ID cho Ngân sách nếu chưa có
            if (string.IsNullOrEmpty(budget.BudgetId))
            {
                budget.BudgetId = "B" + Guid.NewGuid().ToString().Substring(0, 7).ToUpper();
            }

            // 2. QUAN TRỌNG: Dán cái mã m vừa COPY ở Bước 1 vào đây
            // Thay "Dán_Mã_Thật_Ở_Đây" bằng mã m lấy được từ SQL
            budget.UserId = "2";

            // 3. Fix lỗi monthYear nếu m không nhập từ giao diện
            // 3. Fix lỗi monthYear: Vì của m là kiểu String nên phải chuyển sang chuỗi
            if (string.IsNullOrEmpty(budget.MonthYear))
            {
                // Gán mặc định là tháng/năm hiện tại theo định dạng chuỗi
                budget.MonthYear = DateTime.Now.ToString("MM/yyyy");
            }

            // 4. Xóa kiểm tra lỗi Validation cho các bảng liên quan để nút Submit hoạt động
            ModelState.Remove("Category");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(budget);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Nếu vẫn lỗi, nó sẽ báo ra đây cho m xem
                    ModelState.AddModelError("", "Lỗi DB: " + ex.InnerException?.Message);
                }
            }

            // Nạp lại dữ liệu cho Dropdown nếu có lỗi
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", budget.CategoryId);
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "FullName", budget.UserId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", budget.CategoryId);
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "FullName", budget.UserId);
            return View(budget);
        }

        // ... Các hàm khác giữ nguyên, chỉ cần sửa SelectList trong POST Edit tương tự như trên ...
    }
}