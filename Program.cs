using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository;
using ShopEProduction.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// onfigure Database Connection
builder.Services.AddDbContext<ShopEproductionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbContext"),
    sqlOptions => sqlOptions.EnableRetryOnFailure())
);

// Add MVC Support
builder.Services.AddControllersWithViews();

// onfigure Session Management
builder.Services.AddDistributedMemoryCache(); // In-memory session store
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});

// Register Repository with DI
builder.Services.AddScoped<IUserRepository, UserRepository>(); // FIXED

var app = builder.Build();

// Enable Session
app.UseSession();

// Enforce HTTPS
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Ensure Routes Are Configured Correctly
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}"
);

app.Run();
