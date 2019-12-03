using System;
using System.Collections.Generic;
using System.Text;

namespace MintPlayer.Data.Entities
{
	internal class Like
	{
		public Like()
		{
		}

		public Like(Subject subject, User user, bool like) : this()
		{
			if (subject == null) throw new ArgumentNullException(nameof(subject));
			if (user == null) throw new ArgumentNullException(nameof(user));

			Subject = subject;
			SubjectId = subject?.Id ?? 0;
			User = user;
			UserId = user.Id;
			DoesLike = like;
		}

		public int SubjectId { get; set; }
		public Subject Subject { get; set; }

		public Guid UserId { get; set; }
		public User User { get; set; }

		public bool DoesLike { get; set; }
	}
}
