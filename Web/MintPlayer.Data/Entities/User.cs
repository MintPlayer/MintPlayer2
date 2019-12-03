using System;
using Microsoft.AspNetCore.Identity;

namespace MintPlayer.Data.Entities
{
	internal class User : IdentityUser<Guid>
	{
		public string PictureUrl { get; set; }
	}
}
