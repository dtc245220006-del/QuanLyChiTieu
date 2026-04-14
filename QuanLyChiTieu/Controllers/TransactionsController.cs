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
    public class TransactionsController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public TransactionsController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            // Include để lấy dữ liệu từ bảng Category và Wallet
            var quanLyChiTieuContext = _context.Transactions.Include(t => t.Category).Include(t => t.Wallet);
            return View(await quanLyChiTieuContext.ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(m => m.TransactionId == id);

            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            // SỬA TẠI ĐÂY: Hiển thị CategoryName và WalletName thay vì ID
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["WalletId"] = new SelectList(_context.Wallets, "WalletId", "WalletName");
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", transaction.CategoryId);
            ViewData["WalletId"] = new SelectList(_context.Wallets, "WalletId", "WalletName", transaction.WalletId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TransactionId,WalletId,CategoryId,Amount,Note,TransactionDate")] Transaction transaction)
        {
            if (id == null || id != transaction.TransactionId)
            {
                return BadRequest();
            }

            // Remove navigation properties from model validation and prevent EF from tracking them here
            ModelState.Remove("Category");
            ModelState.Remove("Wallet");
            ModelState.Remove("User");
            transaction.Category = null;
            transaction.Wallet = null;

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Transactions.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update only allowed fields to avoid accidental key changes
                    existing.WalletId = transaction.WalletId;
                    existing.CategoryId = transaction.CategoryId;
                    existing.Amount = transaction.Amount;
                    existing.Note = transaction.Note;
                    existing.TransactionDate = transaction.TransactionDate;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu Database: " + ex.Message);
                }
            }

            // If we get here something failed; repopulate selects and return view
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", transaction.CategoryId);
            ViewData["WalletId"] = new SelectList(_context.Wallets, "WalletId", "WalletName", transaction.WalletId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(m => m.TransactionId == id);

            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // ... Các hàm HttpPost Create và Delete giữ nguyên logic cũ ...

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,WalletId,CategoryId,Amount,Note,TransactionDate")] Transaction transaction)
        {
            // 1. Tự sinh ID nếu trống (Cái này m đã có nhưng t viết lại cho chắc)
            if (string.IsNullOrEmpty(transaction.TransactionId))
            {
                transaction.TransactionId = "TR" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            }

            // 2. ÉP BUỘC VƯỢT QUA KIỂM TRA (Sửa lỗi bấm nút không ăn)
            // Xóa kiểm tra lỗi của các đối tượng liên quan vì mình chỉ cần ID thôi
            ModelState.Remove("Category");
            ModelState.Remove("Wallet");
            ModelState.Remove("User");

            // Ensure navigation properties are null so EF won't try to track an entity with a null key
            transaction.Category = null;
            transaction.Wallet = null;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Nếu lỗi DB thì nó sẽ hiện ra đây
                    ModelState.AddModelError("", "Lỗi lưu Database: " + ex.Message);
                }
            }

            // Nếu đến được đây nghĩa là có lỗi gì đó, nạp lại SelectList để hiện lại Form
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", transaction.CategoryId);
            ViewData["WalletId"] = new SelectList(_context.Wallets, "WalletId", "WalletName", transaction.WalletId);
            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null) _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(string id) => _context.Transactions.Any(e => e.TransactionId == id);
    }
}