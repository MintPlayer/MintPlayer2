using System;
using System.Collections.Generic;
using SitemapXml.Interfaces;

namespace MintPlayer.Data.Dtos
{
	public abstract class Subject : ITimestamps
	{
		public int Id { get; set; }
		public abstract string Text { get; }

		[Nest.Ignore]
		public DateTime DateUpdate { get; set; }

		public List<Medium> Media { get; set; }
	}
}
