using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;

using Microsoft.AspNetCore.Authentication.Cookies;

// Create builder first
var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ Cookie
Microsoft.AspNetCore.Authentication.AuthenticationBuilder authenticationBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn bị đuổi về nếu chưa đăng nhập
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Giữ đăng nhập 7 ngày
    });

builder.Services.AddDbContext<QuanLyChiTieuContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
// Thêm vào sau dòng builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Ensure authentication middleware runs before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    // Chỗ này này: Đổi Home thành Account, Index thành Login
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Insert this before app.Run(); it seeds an admin user if not present.
// Requires the UserAccount.Role column to exist in the DB.
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<QuanLyChiTieuContext>();

    if (!ctx.UserAccounts.Any(u => u.Email == "admin@local"))
    {
        ctx.UserAccounts.Add(new UserAccount
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "Admin",
            Email = "admin@local",
            Password = "admin123", // insecure: see notes
            Role = "Admin"
        });
        ctx.SaveChanges();
    }
}

app.Run();