using ITAM.Data;
using ITAM.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ItamDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("KonekToDB")));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<AssetLogService>();
builder.Services.AddScoped<ServiceReport>();
builder.Services.AddScoped<InspectionService>();
builder.Services.AddScoped<ServiceDepartment>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Jika belum login, otomatis dilempar ke sini
        options.AccessDeniedPath = "/Account/AccessDenied"; // Jika akses dilarang
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Sesi login aktif selama 2 jam
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
     name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
