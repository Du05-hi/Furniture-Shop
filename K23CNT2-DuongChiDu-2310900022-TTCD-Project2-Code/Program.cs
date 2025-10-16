using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Cấu hình MVC
builder.Services.AddControllersWithViews();

// ✅ 2. Kết nối SQL Server (đọc từ appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
        sql => sql.EnableRetryOnFailure()
    )
);

// ✅ 3. Cấu hình đăng nhập bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Denied";
    });

// ✅ 4. Bật Session (để lưu mã giảm giá, giỏ hàng tạm, v.v.)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // cho phép session hoạt động kể cả khi user chưa chấp nhận cookie
});

var app = builder.Build();

// ✅ 5. KHÔNG migrate/seed khi khởi động (an toàn khi nộp báo cáo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.SetCommandTimeout(180);
    Console.WriteLine("ℹ️ Using existing FurnitureShopDB — skipping auto migration and seeding.");
}

// ✅ 6. Middleware xử lý ngoại lệ & bảo mật
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ 7. Pipeline chính
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thứ tự quan trọng
app.UseAuthentication();
app.UseAuthorization();

// ✅ 8. Kích hoạt Session
app.UseSession();

// ✅ 9. Định tuyến mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ✅ 10. Chạy ứng dụng
app.Run();
