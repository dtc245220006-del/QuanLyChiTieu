using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyChiTieu.Controllers
{
    // Allow authenticated users; method-level role gating removed so Edit/Delete are reachable
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public CategoriesController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            // Provide a default Type if you want (optional)
            return View(new Category { Type = "Expense" });
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Type")] Category category)
        {
            // --- THÊM ĐOẠN NÀY ---
            if (string.IsNullOrEmpty(category.CategoryId))
            {
                // Tạo mã ngẫu nhiên khoảng 8 ký tự, ví dụ: CAT-a1b2c3d4
                category.CategoryId = "CAT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            }
            // ---------------------

            // Giữ nguyên logic đổi THU/CHI hôm nọ t bảo m
            if (category.Type == "Expense") category.Type = "CHI";
            else if (category.Type == "Income") category.Type = "THU";

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CategoryId,CategoryName,Type")] Category category)
{
    // --- THÊM ĐOẠN NÀY ---
    if (string.IsNullOrEmpty(category.CategoryId))
    {
        // Tạo mã ngẫu nhiên khoảng 8 ký tự, ví dụ: CAT-a1b2c3d4
        category.CategoryId = "CAT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
    // ---------------------

    // Giữ nguyên logic đổi THU/CHI hôm nọ t bảo m
    if (category.Type == "Expense") category.Type = "CHI";
    else if (category.Type == "Income") category.Type = "THU";

    if (ModelState.IsValid)
    {
        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(category);
}

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null) _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(string id) => _context.Categories.Any(e => e.CategoryId == id);
    }
}
