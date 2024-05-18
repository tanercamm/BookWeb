using System.ComponentModel.DataAnnotations;

public class CommentViewModel
{
	[Key]
	public int CommentId { get; set; }

	[Required]
	[StringLength(500, MinimumLength = 50, ErrorMessage = "Comment text must be between 50-500 characters.")]
	public string Text { get; set; }

	[Required]
	public int BookId { get; set; }

	public string AuthorFullName { get; set; }

	// Kitap Bilgileri
	public string Title { get; set; }
	public string Publisher { get; set; }
	public string ImageUrl { get; set; }

	// Kullanıcı Bilgileri
	public string UserId { get; set; }
	public string UserName { get; set; }
	public string FullName { get; set; }
	public string Email { get; set; }
	public string UserImageUrl { get; set; }

    public DateTime DateTime { get; set; }
}
