using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionStr = builder.Configuration.GetConnectionString("db");
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<MarketManagementContext>(options =>
{
	options.UseSqlServer(connectionStr);

});
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
	options =>
	{
		options.LoginPath = new PathString("/dang-nhap");
		options.AccessDeniedPath = new PathString("/");
	});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
			name: "areas",
			pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
		  );
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
