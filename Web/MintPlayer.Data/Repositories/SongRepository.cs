﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Helpers;
using MintPlayer.Data.Repositories.Interfaces;

namespace MintPlayer.Data.Repositories
{
	internal class SongRepository : ISongRepository
	{
		private IHttpContextAccessor http_context;
		private MintPlayerContext mintplayer_context;
		private UserManager<Entities.User> user_manager;
		private SongHelper song_helper;
		private Jobs.Interfaces.IElasticSearchJobRepository elasticSearchJobRepository;
		public SongRepository(IHttpContextAccessor http_context, MintPlayerContext mintplayer_context, UserManager<Entities.User> user_manager, SongHelper song_helper, Jobs.Interfaces.IElasticSearchJobRepository elasticSearchJobRepository)
		{
			this.http_context = http_context;
			this.mintplayer_context = mintplayer_context;
			this.user_manager = user_manager;
			this.song_helper = song_helper;
			this.elasticSearchJobRepository = elasticSearchJobRepository;
		}

		public IEnumerable<Dtos.Song> GetSongs(bool include_relations = false)
		{
			if (include_relations)
			{
				var songs = mintplayer_context.Songs
					.Include(song => song.Lyrics)
					.Include(song => song.Artists)
						.ThenInclude(@as => @as.Artist)
					.Include(song => song.Media)
						.ThenInclude(m => m.Type)
					.Select(song => ToDto(song, true));
				return songs;
			}
			else
			{
				var songs = mintplayer_context.Songs
					//.Include(song => song.Lyrics)
					.Select(song => ToDto(song, false));
				return songs;
			}
		}

		public Dtos.Song GetSong(int id, bool include_relations = false)
		{
			if (include_relations)
			{
				var song = mintplayer_context.Songs
					.Include(s => s.Lyrics)
					.Include(s => s.Artists)
						.ThenInclude(@as => @as.Artist)
					.Include(s => s.Media)
						.ThenInclude(m => m.Type)
					.SingleOrDefault(s => s.Id == id);
				return ToDto(song, true);
			}
			else
			{
				var song = mintplayer_context.Songs
					.Include(s => s.Lyrics)
					.SingleOrDefault(s => s.Id == id);
				return ToDto(song, false);
			}
		}

		public async Task<Song> InsertSong(Song song)
		{
			// Get current user
			var user = await user_manager.GetUserAsync(http_context.HttpContext.User);

			// Convert to entity
			var entity_song = ToEntity(song, user, mintplayer_context);
			entity_song.UserInsert = user;
			entity_song.InsertedAt = DateTime.Now;

			// Add to database
			mintplayer_context.Songs.Add(entity_song);
			await mintplayer_context.SaveChangesAsync();

			// Index
			var new_song = ToDto(entity_song);
			//var index_status = await elastic_client.IndexDocumentAsync(new_song);
			var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
			{
				Subject = new_song,
				SubjectStatus = Enums.eSubjectAction.Added,
				JobStatus = Enums.eJobStatus.Queued
			});

			return new_song;
		}

		public async Task<Song> UpdateSong(Song song)
		{
			// Find existing song
			var song_entity = mintplayer_context.Songs.Include(s => s.Artists)
				.Include(s => s.Lyrics)
				.SingleOrDefault(s => s.Id == song.Id);

			// Set new properties
			song_entity.Title = song.Title;
			song_entity.Released = song.Released;

			// Add/update/delete artists
			IEnumerable<Entities.ArtistSong> to_add, to_remove, to_update;
			song_helper.CalculateUpdatedArtists(song_entity, song, mintplayer_context, out to_add, out to_update, out to_remove);
			foreach (var item in to_remove)
				mintplayer_context.Entry(item).State = EntityState.Deleted;
			foreach (var item in to_add)
				mintplayer_context.Entry(item).State = EntityState.Added;
			foreach (var item in to_update)
				mintplayer_context.Entry(item).State = EntityState.Modified;

			// Set UserUpdate
			var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
			song_entity.UserUpdate = user;
			song_entity.UpdatedAt = DateTime.Now;

			// Add/update lyrics
			var lyrics = song_entity.Lyrics.FirstOrDefault(l => l.UserId == user.Id);
			if (lyrics == null)
			{
				lyrics = new Entities.Lyrics(user, song_entity);
				lyrics.Text = song.Lyrics;
				lyrics.UpdatedAt = DateTime.Now;
				mintplayer_context.Entry(lyrics).State = EntityState.Added;
			}
			else
			{
				lyrics.Text = song.Lyrics;
				lyrics.UpdatedAt = DateTime.Now;
				mintplayer_context.Entry(lyrics).State = EntityState.Modified;
			}

			// Update
			mintplayer_context.Entry(song_entity).State = EntityState.Modified;

			// Index
			var updated_song = ToDto(song_entity);
			//await elastic_client.UpdateAsync<Song>(updated_song, u => u.Doc(updated_song));
			var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
			{
				Subject = updated_song,
				SubjectStatus = Enums.eSubjectAction.Updated,
				JobStatus = Enums.eJobStatus.Queued
			});

			return updated_song;
		}

		public async Task DeleteSong(int song_id)
		{
			// Find existing song
			var song = mintplayer_context.Songs.Find(song_id);

			// Get current user
			var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
			song.UserDelete = user;
			song.DeletedAt = DateTime.Now;

			// Index
			var deleted_song = ToDto(song);
			//await elastic_client.DeleteAsync<Song>(deleted_song);
			var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
			{
				Subject = deleted_song,
				SubjectStatus = Enums.eSubjectAction.Deleted,
				JobStatus = Enums.eJobStatus.Queued
			});
		}

		public async Task SaveChangesAsync()
		{
			await mintplayer_context.SaveChangesAsync();
		}

		#region Conversion methods
		internal static Song ToDto(Entities.Song song, bool include_relations = false)
		{
			if (song == null) return null;
			if (include_relations)
			{
				return new Song
				{
					Id = song.Id,
					Title = song.Title,
					Released = song.Released,
					Lyrics = song.Lyrics.OrderBy(l => l.UpdatedAt).LastOrDefault()?.Text,

					DateUpdate = song.UpdatedAt ?? song.InsertedAt,

					Artists = song.Artists.Select(@as => ArtistRepository.ToDto(@as.Artist)).ToList(),
					Media = song.Media.Select(medium => MediumRepository.ToDto(medium, true)).ToList()
				};
			}
			else
			{
				return new Song
				{
					Id = song.Id,
					Title = song.Title,
					Released = song.Released,
					Lyrics = song.Lyrics?.OrderBy(l => l.UpdatedAt).LastOrDefault()?.Text,

					DateUpdate = song.UpdatedAt ?? song.InsertedAt,
				};
			}
		}
		/// <summary>Only use this method for creation of a song</summary>
		internal static Entities.Song ToEntity(Song song, Entities.User user, MintPlayerContext mintplayer_context)
		{
			if (song == null) return null;
			var entity_song = new Entities.Song
			{
				Id = song.Id,
				Title = song.Title,
				Released = song.Released
			};
			entity_song.Artists = song.Artists.Select(artist =>
			{
				var entity_artist = mintplayer_context.Artists.Find(artist.Id);
				return new Entities.ArtistSong(entity_artist, entity_song);
			}).ToList();
			entity_song.Lyrics = new List<Entities.Lyrics>(new[] {
				new Entities.Lyrics { Song = entity_song, User = user, Text = song.Lyrics }
			});
			entity_song.Media = song.Media.Select(m => {
				var medium = MediumRepository.ToEntity(m, mintplayer_context);
				medium.Subject = entity_song;
				return medium;
			}).ToList();
			return entity_song;
		}
		#endregion
	}
}
