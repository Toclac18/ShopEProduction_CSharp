using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShopEproductionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbContext"),
    sqlOptions => sqlOptions.EnableRetryOnFailure())
);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // In-memory session store
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});

var app = builder.Build();

app.UseSession(); // Enable session middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
