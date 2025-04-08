using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository;
using ShopEProduction.Repository.IRepository;
using ShopEProduction.Services.Email;
using ShopEProduction.Services.Email.IService;
using ShopEProduction.Services.Files;
using ShopEProduction.Services.OTP;
using ShopEProduction.Services.OTP.IOTP;
using ShopEProduction.Services.SMS;
using ShopEProduction.Services.SMS.ISMS;

var builder = WebApplication.CreateBuilder(args);

// Explicitly register IWebHostEnvironment as a singleton
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Configure Database Connection
builder.Services.AddDbContext<ShopEproductionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbContext"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())
);

// Add MVC Support
builder.Services.AddControllersWithViews();

// Configure Session Management
builder.Services.AddDistributedMemoryCache(); // In-memory session store
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});
// Register Service with DI
builder.Services.AddAntiforgery();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped<ISmsService, TwilioSmsService>();
// Register Repository with DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

// Debug environment at startup (using the final service provider)
var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Enable Session
app.UseSession();

// Enforce HTTPS
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles(); // Serves static files like images, CSS, JS from the wwwroot folder

// Debug environment after app build
var env = app.Services.GetService<IWebHostEnvironment>();
Console.WriteLine($"After Build - Environment: {env?.EnvironmentName}, WebRootPath: {env?.WebRootPath}");

// Ensure Routes Are Configured Correctly
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}"
);

app.Run();