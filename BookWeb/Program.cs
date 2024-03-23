using BookWeb.Data;
using BookWeb.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BookContext>(options =>
{
    //var config = builder.Configuration;
    //var connectionString = config.GetConnectionString("mssql_connection");
    //options.UseSqlServer(connectionString);
    var connectionString = builder.Configuration.GetConnectionString("mssql_connection") + ";MultipleActiveResultSets=True";  // ayný anda birden fazla sorgulama sorununu çözmek için yazýyoruz
    options.UseSqlServer(connectionString);
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

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
