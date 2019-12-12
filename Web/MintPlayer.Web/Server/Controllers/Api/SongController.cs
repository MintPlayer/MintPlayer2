using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Repositories.Interfaces;
using MintPlayer.Web.ViewModels.Song;

namespace MintPlayer.Web.Controllers.Api
{
	[Route("api/[controller]")]
	public class SongController : Controller
	{
		private ISongRepository songRepository;
		private IMediumRepository mediumRepository;
		public SongController(ISongRepository songRepository, IMediumRepository mediumRepository)
		{
			this.songRepository = songRepository;
			this.mediumRepository = mediumRepository;
		}

		// GET: api/Song
		[HttpGet(Name = "api-song-list")]
		public IEnumerable<Song> Get([FromHeader]bool include_relations = false)
		{
			var songs = songRepository.GetSongs(include_relations);
			return songs.ToList();
		}
		// GET: api/Song/5
		[HttpGet("{id}", Name = "api-song-get", Order = 1)]
		public Song Get(int id, [FromHeader]bool include_relations = false)
		{
			var song = songRepository.GetSong(id, include_relations);
			return song;
		}
		// POST: api/Song
		[HttpPost(Name = "api-song-create")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<Song> Post([FromBody] SongCreateVM songCreateVM)
		{
			var song = await songRepository.InsertSong(songCreateVM.Song);
			await mediumRepository.StoreMedia(songCreateVM.Song, songCreateVM.Song.Media);
			await songRepository.SaveChangesAsync();
			return song;
		}
		// PUT: api/Song/5
		[HttpPut("{id}", Name = "api-song-update")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task Put(int id, [FromBody] SongUpdateVM songUpdateVM)
		{
			await songRepository.UpdateSong(songUpdateVM.Song);
			await mediumRepository.StoreMedia(songUpdateVM.Song, songUpdateVM.Song.Media);
			await songRepository.SaveChangesAsync();
		}
		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}", Name = "api-song-delete")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task Delete(int id)
		{
			await songRepository.DeleteSong(id);
			await songRepository.SaveChangesAsync();
		}
	}
}