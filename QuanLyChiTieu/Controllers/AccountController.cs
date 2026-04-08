using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QuanLyChiTieu.Models; // Sửa lại đúng tên project của m
using System.Linq;

namespace QuanLyChiTieu.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public AccountController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // ================= ĐĂNG KÝ =================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(UserAccount user)
        {
            if (_context.UserAccounts.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Email này đã được sử dụng!";
                return View();
            }

            // ensure PK is set server-side
            user.UserId = Guid.NewGuid().ToString();

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("Login");
        }

        // ================= ĐĂNG NHẬP =================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Tìm user trong DB có khớp email và pass không
            var user = _context.UserAccounts.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Tạo "Thẻ căn cước" (Claims) cho user này
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Cấp phát Cookie và cho qua cửa
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home"); // Chuyển về trang chủ
            }

            ViewBag.Error = "Sai email hoặc mật khẩu!";
            return View();
        }

        // ================= ĐĂNG XUẤT =================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}