﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MintPlayer.Data.Repositories.Interfaces;
using MintPlayer.Web.ViewModels.Subject;
using System.Collections.Generic;
using MintPlayer.Data.Dtos;
using System.Linq;

namespace MintPlayer.Web.Controllers.Web
{
	[ApiController]
	[Route("web/[controller]")]
	public class SubjectController : Controller
	{
		private ISubjectRepository subjectRepository;
		public SubjectController(ISubjectRepository subjectRepository)
		{
			this.subjectRepository = subjectRepository;
		}

		[HttpGet("{subject_id}/likes", Name = "web-subject-getlikes")]
		public async Task<SubjectLikeVM> Likes([FromRoute]int subject_id)
		{
			var likes = await subjectRepository.GetLikes(subject_id);
			bool authenticated; bool? doeslike;
			try
			{
				doeslike = await subjectRepository.DoesLike(subject_id);
				authenticated = true;
			}
			catch (UnauthorizedAccessException)
			{
				doeslike = null;
				authenticated = false;
			}

			return new SubjectLikeVM
			{
				Likes = likes.Item1,
				Dislikes = likes.Item2,
				Like = doeslike,
				Authenticated = authenticated
			};
		}

		[Authorize]
		[HttpPost("{subject_id}/likes", Name = "web-subject-like")]
		public async Task<SubjectLikeVM> Like([FromRoute]int subject_id, [FromBody]bool like)
		{
			await subjectRepository.Like(subject_id, like);
			await subjectRepository.SaveChangesAsync();
			var likes = await subjectRepository.GetLikes(subject_id);
			var doeslike = await subjectRepository.DoesLike(subject_id);
			return new SubjectLikeVM
			{
				Likes = likes.Item1,
				Dislikes = likes.Item2,
				Like = doeslike,
				Authenticated = true
			};
		}

		[HttpGet("search/suggest/{subjects_concat}/{search_term}", Name = "web-subject-suggest")]
		public async Task<IEnumerable<Subject>> Suggest([FromRoute]string subjects_concat, [FromRoute]string search_term)
		{
			// Still to do:
			// https://devadventures.net/2018/05/03/implementing-autocomplete-and-more-like-this-using-asp-net-core-elasticsearch-and-nest-5-x-part-4-4/

			var subjects = subjects_concat.Split('-');
			var valid_subjects = new[] { "artist", "person", "song" };

			if (subjects.FirstOrDefault() == null)
				subjects = valid_subjects;
			else if (subjects.FirstOrDefault() == "all")
				subjects = valid_subjects;
			else if (!subjects.Intersect(valid_subjects).Any())
				subjects = valid_subjects;
			else
				subjects = subjects.Intersect(valid_subjects).ToArray();

			var results = await subjectRepository.Suggest(subjects, search_term);
			return results.ToList();
		}

		[HttpGet("search/{subjects_concat}/{search_term}", Name = "web-subject-search")]
		public async Task<SearchResultsVM> Search([FromRoute]string subjects_concat, [FromRoute]string search_term)
		{
			var subjects = subjects_concat.Split('-');
			var valid_subjects = new[] { "artist", "person", "song" };

			if (subjects.FirstOrDefault() == null)
				subjects = valid_subjects;
			else if (subjects.FirstOrDefault() == "all")
				subjects = valid_subjects;
			else if (!subjects.Intersect(valid_subjects).Any())
				subjects = valid_subjects;
			else
				subjects = subjects.Intersect(valid_subjects).ToArray();

			var results = await subjectRepository.Search(subjects, search_term, false);
			return new SearchResultsVM
			{
				Artists = results.Where(s => s.GetType() == typeof(Artist)).Cast<Artist>().ToList(),
				People = results.Where(s => s.GetType() == typeof(Person)).Cast<Person>().ToList(),
				Songs = results.Where(s => s.GetType() == typeof(Song)).Cast<Song>().ToList()
			};
		}
	}
}