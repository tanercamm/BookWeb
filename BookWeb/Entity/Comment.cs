using System.ComponentModel.DataAnnotations;

namespace BookWeb.Entity
{
	public class Comment
	{
		[Key]
		public int CommentId { get; set; }
		public string Text { get; set; }
		public DateTime DateTime { get; set; }

		public int BookId { get; set; }
		public Book Book { get; set; }

		public string UserId { get; set; }
		public AppUser User { get; set; }

	}
}
