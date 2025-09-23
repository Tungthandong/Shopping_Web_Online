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
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
