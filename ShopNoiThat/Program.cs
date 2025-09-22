using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopNoiThat.Models;

namespace ShopNoiThat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Kết nối SQL Server
            var connectionString = builder.Configuration.GetConnectionString("NoiThatDbConnect");
            builder.Services.AddDbContext<ShopNoiThatDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Nếu bạn muốn dùng Identity chuẩn, bạn có thể thêm:
            // builder.Services.AddIdentity<User, IdentityRole>()
            //     .AddEntityFrameworkStores<ShopNoiThatDbContext>()
            //     .AddDefaultTokenProviders();

            var app = builder.Build();

            // Tạo admin mặc định
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<ShopNoiThatDbContext>();

                // Nếu dùng migrations để tạo bảng, bỏ comment dòng này:
                // db.Database.Migrate();

                if (!db.Users.Any(u => u.UserName == "admin"))
                {
                    var hasher = new PasswordHasher<User>();
                    var admin = new User
                    {
                        UserName = "admin",
                        FullName = "Quản trị viên",
                        Email = "admin@shop.com",
                        Role = "Admin"
                    };
                    admin.PasswordHash = hasher.HashPassword(admin, "Admin@123"); // mật khẩu mặc định
                    db.Users.Add(admin);
                    db.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
