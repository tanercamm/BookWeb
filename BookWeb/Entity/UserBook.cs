using System.ComponentModel.DataAnnotations;

namespace BookWeb.Entity
{
	public class UserBook
	{
		[Key]
		public string UserId { get; set; }
		public AppUser User { get; set; }

		[Key]
		public int BookId { get; set; }
		public Book Book { get; set; }

		public bool IsActive { get; set; }

	}
}
