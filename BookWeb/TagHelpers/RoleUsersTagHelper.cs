using BookWeb.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace BookWeb.TagHelpers
{
	[HtmlTargetElement("td", Attributes = "asp-role-users")]
	public class RoleUsersTagHelper : TagHelper
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		[HtmlAttributeName("asp-role-users")]
		public string RoleId { get; set; } = null!;



		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var userNames = new List<string>();

			// RoleId'nin boş olup olmadığını kontrol et
			if (!string.IsNullOrEmpty(RoleId))
			{
				// Role'ü bul
				var role = await _roleManager.FindByIdAsync(RoleId);

				// Role bulunduysa devam et
				if (role != null && role.Name != null)
				{
					// Kullanıcıları al
					var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

					// Kullanıcı adlarını listeye ekle
					foreach (var user in usersInRole)
					{
						userNames.Add(user.UserName ?? "");
					}

					// Kullanıcı adlarını içeren HTML'i oluştur
					var userListHtml = userNames.Count == 0 ? "Kullanıcı yok" : setHtml(userNames);
					output.Content.SetHtmlContent(userListHtml);
				}
			}
		}




		private string setHtml(List<string> userNames)
		{
			var html = "<ul>";
			foreach (var item in userNames)
			{
				html += "<li>" + item + "</li>";
			}
			html += "</ul>";
			return html;
		}


	}
}
