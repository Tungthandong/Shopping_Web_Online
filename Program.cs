using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Shopping_Web.DataAccess;
using Shopping_Web.Models;
using Shopping_Web.Services;
using Shopping_Web.Services.Vnpay;

namespace Shopping_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Đọc đường dẫn file key từ appsettings
            var googleCredentialsPath = builder.Configuration["GoogleApplicationCredentials"];
            if (!string.IsNullOrEmpty(googleCredentialsPath))
            {
                // Set biến môi trường để thư viện Google tự động tìm thấy
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", googleCredentialsPath);
            }

            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<YugiohCardShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));
            builder.Services.AddScoped<IProductDA, ProductDA>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryDA, CategoryDA>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAccountDA, AccountDA>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICartDA, CartDA>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderDA, OrderDA>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            // Thêm IHttpClientFactory
            builder.Services.AddHttpClient();
            builder.Configuration.AddUserSecrets<Program>();
            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");



            app.Run();
        }
    }
}
