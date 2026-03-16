using EnglishLearning.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 🟢 Bật upload lớn cho Kestrel (200MB)
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 209_715_200; // 200MB
            });

            // Add services to the container.
            builder.Services.AddDbContext<EnglishLearningDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpClient();
            // ✅ Cấu hình giới hạn upload (200MB)
            builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;      // Kích thước tối đa mỗi input
                options.ValueCountLimit = int.MaxValue;       // Số lượng input cho phép
                options.MultipartBodyLengthLimit = 209_715_200; // 200MB cho upload file
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            // ✅ Thêm cấu hình Authentication với 2 schemes riêng biệt
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Google"; // dùng Google khi cần challenge
            })
            // Cookie cho User (trang chính)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "UserAuth";
            })
            // Cookie riêng cho Admin
            .AddCookie("AdminScheme", options =>
            {
                options.LoginPath = "/Admin/Auth/Login";
                options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
                options.Cookie.Name = "AdminAuth";
            })
            .AddGoogle("Google", options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            }).AddFacebook(options =>
            {
                options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                options.Events.OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/Login");
                    context.HandleResponse();
                    return Task.CompletedTask;
                };
            });



            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            // ✅ Bắt buộc: gọi authentication trước authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
