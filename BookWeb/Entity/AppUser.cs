using Microsoft.AspNetCore.Identity;

namespace BookWeb.Entity
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ImageUrl { get; set; }
		public List<UserBook> Books { get; set; }

	}
}
