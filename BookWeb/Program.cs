using BookWeb.Data;
using BookWeb.Entity;
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
	var connectionString = builder.Configuration.GetConnectionString("mssql_connection") + ";MultipleActiveResultSets=True";  // ayný anda birden fazla sorgulama sorununu çözmek için yazýyoruz
	options.UseSqlServer(connectionString);
});

// Identity ekliyoruz
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<BookContext>().AddDefaultTokenProviders();

// gerekli kurallarý tanýmlýyoruz
builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireDigit = false;

	// Email sadece bir kiþiye tanýmlý olsun = true;
	options.User.RequireUniqueEmail = true;

	// içerdiði karakter dizisi
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // hatalý giriþte 5 dakika süre cezasý
	options.Lockout.MaxFailedAccessAttempts = 5;   // girmek için 5 kez hakký bulunur
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
	pattern: "{controller=Home}/{action=Index}/{id?}");

// verileri çalýþmadan önce tanýmlamasý için komutu yazýyoruz
SeedData.IdentityTestUser(app);
// build oldu, veriler yüklendi ve þimdi de run edilecek

app.Run();
