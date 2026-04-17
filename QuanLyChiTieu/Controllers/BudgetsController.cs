using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyChiTieu.Controllers
{
    // Require authenticated users for budgets; admins can be allowed additional actions if needed
    [Authorize]
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
            // SỬA TẠI ĐÂY: Hiển thị CategoryName và Username thay vì ID
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "Username"); // fixed: use existing property
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

            // 2. Fix MonthYear nếu không nhập từ giao diện
            if (string.IsNullOrEmpty(budget.MonthYear))
            {
                budget.MonthYear = DateTime.Now.ToString("MM/yyyy");
            }

            // 3. Determine a valid UserId (do NOT hard-code)
            // Prefer the posted UserId if present and exists in DB
            string resolvedUserId = null;
            if (!string.IsNullOrEmpty(budget.UserId))
            {
                var exists = await _context.UserAccounts.AnyAsync(u => u.UserId == budget.UserId);
                if (exists) resolvedUserId = budget.UserId;
            }

            // If no valid posted UserId, try to use current logged-in user
            if (resolvedUserId == null)
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;
                if (!string.IsNullOrEmpty(claim))
                {
                    var user = await _context.UserAccounts
                        .FirstOrDefaultAsync(u => u.UserId == claim || u.Username == claim || u.Email == claim);
                    if (user != null) resolvedUserId = user.UserId;
                }
            }

            // Fallback: use any existing user in DB (so FK constraint is satisfied)
            if (resolvedUserId == null)
            {
                resolvedUserId = await _context.UserAccounts.Select(u => u.UserId).FirstOrDefaultAsync();
            }

            if (string.IsNullOrEmpty(resolvedUserId))
            {
                // No user exists in DB -> cannot create budget
                ModelState.AddModelError("", "No user account available. Create a user first or select an owner.");
            }
            else
            {
                budget.UserId = resolvedUserId;
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
                    ModelState.AddModelError("", "Lỗi DB: " + ex.InnerException?.Message ?? ex.Message);
                }
            }

            // Nạp lại dữ liệu cho Dropdown nếu có lỗi
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", budget.CategoryId);
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "Username", budget.UserId); // fixed: use existing property
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", budget.CategoryId);
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "Username", budget.UserId); // fixed: use existing property
            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BudgetId,UserId,CategoryId,LimitAmount,MonthYear")] Budget budget)
        {
            if (id != budget.BudgetId) return NotFound();

            // 3. Fix lỗi monthYear: Vì của m là kiểu String nên phải chuyển sang chuỗi
            if (string.IsNullOrEmpty(budget.MonthYear))
            {
                // Gán mặc định là tháng/năm hiện tại theo định dạng chuỗi
                budget.MonthYear = DateTime.Now.ToString("MM/yyyy");
            }

            // Xóa kiểm tra lỗi Validation cho các bảng liên quan để nút Submit hoạt động
            ModelState.Remove("Category");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    // Không làm thay đổi UserId và MonthYear trong quá trình chỉnh sửa
                    var existingBudget = await _context.Budgets.AsNoTracking().FirstOrDefaultAsync(b => b.BudgetId == id);
                    budget.UserId = existingBudget.UserId;
                    budget.MonthYear = existingBudget.MonthYear;

                    _context.Update(budget);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.BudgetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi DB: " + ex.InnerException?.Message);
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", budget.CategoryId);
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "Username", budget.UserId); // fixed: use existing property
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BudgetId == id);

            if (budget == null) return NotFound();
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetExists(string id)
        {
            return _context.Budgets.Any(e => e.BudgetId == id);
        }
    }
}