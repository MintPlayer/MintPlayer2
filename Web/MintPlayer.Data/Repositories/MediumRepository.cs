﻿using MintPlayer.Data.Dtos;
using MintPlayer.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintPlayer.Data.Repositories
{
	internal class MediumRepository : IMediumRepository
	{
		private MintPlayerContext mintplayer_context;
		public MediumRepository(MintPlayerContext mintplayer_context)
		{
			this.mintplayer_context = mintplayer_context;
		}

		public async Task StoreMedia(Subject subject, IEnumerable<Medium> media)
		{
			// Remove the old media
			foreach (var medium in mintplayer_context.Media.Where(m => m.Subject.Id == subject.Id))
				mintplayer_context.Media.Remove(medium);

			// Get entity from database
			var entity_subject = await mintplayer_context.Subjects.FindAsync(subject.Id);
			foreach (var medium in media)
			{
				var entity_medium = new Entities.Medium
				{
					Subject = entity_subject,
					Type = await mintplayer_context.MediumTypes.FindAsync(medium.Type.Id),
					Value = medium.Value
				};
				mintplayer_context.Add(entity_medium);
			}
		}

		internal static Medium ToDto(Entities.Medium medium, bool include_relations = false)
		{
			if (medium == null) return null;
			if (include_relations)
			{
				return new Medium
				{
					Id = medium.Id,
					Type = MediumTypeRepository.ToDto(medium.Type),
					Value = medium.Value
				};
			}
			else
			{
				return new Medium
				{
					Id = medium.Id,
					Value = medium.Value
				};
			}
		}
		internal static Entities.Medium ToEntity(Medium medium, MintPlayerContext mintplayer_context)
		{
			if (medium == null) return null;
			return new Entities.Medium
			{
				Id = medium.Id,
				Type = mintplayer_context.MediumTypes.Find(medium.Type.Id),
				Value = medium.Value
			};
		}
	}
}
