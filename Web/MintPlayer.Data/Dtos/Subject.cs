using SitemapXml.Interfaces;
using System;
using System.Collections.Generic;

namespace MintPlayer.Data.Dtos
{
	public class Subject : ITimestamps
	{
		public int Id { get; set; }

		[Nest.Ignore]
		public DateTime DateUpdate { get; set; }

		public List<Medium> Media { get; set; }
	}
}
