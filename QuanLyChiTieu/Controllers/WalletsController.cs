using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;
using Microsoft.AspNetCore.Authorization; // Thêm cái này ở đầu file


namespace QuanLyChiTieu.Controllers
{
    public class WalletsController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public WalletsController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // GET: Wallets
        public async Task<IActionResult> Index()
        {
            var quanLyChiTieuContext = _context.Wallets.Include(w => w.User);
            return View(await quanLyChiTieuContext.ToListAsync());
        }

        // GET: Wallets/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WalletId == id);
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // GET: Wallets/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "UserId");
            return View();
        }

        // POST: Wallets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WalletId,UserId,WalletName,Balance")] Wallet wallet)
        {
            if (string.IsNullOrEmpty(wallet.WalletId))
            {
                wallet.WalletId = "W" + Guid.NewGuid().ToString().Substring(0, 7).ToUpper();
            }

            // If the form didn't supply UserId, try to resolve current logged-in user
            if (string.IsNullOrEmpty(wallet.UserId))
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;
                if (!string.IsNullOrEmpty(claim))
                {
                    var user = await _context.UserAccounts
                        .FirstOrDefaultAsync(u => u.UserId == claim || u.Username == claim || u.Email == claim);
                    if (user != null) wallet.UserId = user.UserId;
                }
            }

            // Fallback: use any existing user in DB (so FK constraint is satisfied)
            if (string.IsNullOrEmpty(wallet.UserId))
            {
                var anyUserId = await _context.UserAccounts.Select(u => u.UserId).FirstOrDefaultAsync();
                if (string.IsNullOrEmpty(anyUserId))
                {
                    ModelState.AddModelError("", "No user accounts exist. Create a user first or select an owner.");
                }
                else
                {
                    wallet.UserId = anyUserId;
                }
            }

            // Remove navigation validation entries
            ModelState.Remove("User");
            ModelState.Remove("Transactions");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(wallet);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu Database: " + ex.Message);
                }
            }

            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "UserId", wallet.UserId);
            return View(wallet);
        }

        // GET: Wallets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "UserId", wallet.UserId);
            return View(wallet);
        }

        // POST: Wallets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("WalletId,UserId,WalletName,Balance")] Wallet wallet)
        {
            if (id != wallet.WalletId)
            {
                return NotFound();
            }

            // Remove navigation properties from model validation (they are not submitted by the form)
            ModelState.Remove("User");
            ModelState.Remove("Transactions");

            // Null navigation props to avoid EF attempting to attach incomplete objects
            wallet.User = null;
            wallet.Transactions = null;

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Wallets.FindAsync(id);
                    if (existing == null) return NotFound();

                    // Update only allowed fields
                    existing.UserId = wallet.UserId;
                    existing.WalletName = wallet.WalletName;
                    existing.Balance = wallet.Balance;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WalletExists(wallet.WalletId))
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
                    ModelState.AddModelError("", "Lỗi lưu Database: " + ex.Message);
                }
            }
            ViewData["UserId"] = new SelectList(_context.UserAccounts, "UserId", "UserId", wallet.UserId);
            return View(wallet);
        }

        // GET: Wallets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WalletId == id);
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // POST: Wallets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WalletExists(string id)
        {
            return _context.Wallets.Any(e => e.WalletId == id);
        }
    }
}
