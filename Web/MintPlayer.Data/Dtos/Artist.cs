﻿using Nest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MintPlayer.Data.Dtos
{
	public class Artist : Subject
	{
		public string Name { get; set; }
		public int? YearStarted { get; set; }
		public int? YearQuit { get; set; }

		public override string Text => Name;

		public List<Person> PastMembers { get; set; }
		public List<Person> CurrentMembers { get; set; }
		public List<Song> Songs { get; set; }

		[JsonIgnore]
		public CompletionField NameSuggest => new CompletionField { Input = new[] { Name } };
	}
}
