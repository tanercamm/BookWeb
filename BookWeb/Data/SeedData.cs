using BookWeb.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookWeb.Data
{
	public class SeedData
	{
		private const string adminUser = "admin";
		private const string adminPassword = "Admin_123";

		// TestUser
		public static async void IdentityTestUser(IApplicationBuilder app)
		{

			var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<BookContext>();

			// proje her çalıştırıldığında migrate edilme komutunu hazır tutuyoruz
			if (!context.Database.GetAppliedMigrations().Any())
			{
				context.Database.Migrate();  //  <=>  'database update'
			}

			var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();


			var user = await userManager.FindByNameAsync(adminUser);

			if (user == null)
			{
				user = new AppUser
				{
					FullName = "Admin BookWeb",
					UserName = adminUser,
					Email = "info@admin.com",
					ImageUrl = "admin.png"
				};

				await userManager.CreateAsync(user, adminPassword);
			}

		}

	}
}
