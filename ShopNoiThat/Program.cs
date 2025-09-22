using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopNoiThat.Models;
using ShopNoiThat.Data;

namespace ShopNoiThat
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Kết nối SQL Server
            var connectionString = builder.Configuration.GetConnectionString("NoiThatDbConnect");
            builder.Services.AddDbContext<ShopNoiThatDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Thêm Identity (User + Role)
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ShopNoiThatDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Tạo Role + Admin mặc định
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roleNames = { "Admin", "User" };

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Tạo admin mặc định
                var admin = await userManager.FindByNameAsync("admin");
                if (admin == null)
                {
                    admin = new User
                    {
                        UserName = "admin",
                        Email = "admin@shop.com",
                        FullName = "Quản trị viên"
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // quan trọng: thêm xác thực
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
