using BookWeb.Data;
using BookWeb.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database connection
builder.Services.AddDbContext<BookContext>(options =>
{
	//var config = builder.Configuration;
	//var connectionString = config.GetConnectionString("mssql_connection");
	//options.UseSqlServer(connectionString);
	var connectionString = builder.Configuration.GetConnectionString("mssql_connection") + ";MultipleActiveResultSets=True";  // ayn� anda birden fazla sorgulama sorununu ��zmek i�in yaz�yoruz
	options.UseSqlServer(connectionString);
});

// Identity ekliyoruz
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<BookContext>().AddDefaultTokenProviders();

// gerekli kurallar� tan�ml�yoruz
builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireDigit = false;

	// Email sadece bir ki�iye tan�ml� olsun = true;
	options.User.RequireUniqueEmail = true;

	// i�erdi�i karakter dizisi
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // hatal� giri�te 5 dakika s�re cezas�
	options.Lockout.MaxFailedAccessAttempts = 5;   // girmek i�in 5 kez hakk� bulunur
});

// authentication i�in gerekli kod dizisini ekliyoruz. 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
	options.LoginPath = "/Account/Login";
	options.AccessDeniedPath = "/Account/AccessDenied"; // <=> yetkisiz eri�im durumunda yollad��� path yolu
	options.SlidingExpiration = true;               // eri�im sa�land��� vakit s�resi resetler
													//options.ExpireTimeSpan = TimeSpan.FromDays(30);
	options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();

app.UseAuthorization();


// 404 hatas� i�in Error/ErrorPage sayfas�na y�nlendirme
app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
	{
		context.Response.Clear();
		context.Response.Redirect("/Error/ErrorPage");
	}
});


app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// verileri �al��madan �nce tan�mlamas� i�in komutu yaz�yoruz
SeedData.IdentityTestUser(app);
// build oldu, veriler y�klendi ve �imdi de run edilecek

app.Run();
