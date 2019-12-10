using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MintPlayer.Data.Repositories.Interfaces;

namespace MintPlayer.Web.Server.Controllers.Web
{
	[ApiController]
	[Route("web/[controller]")]
	public class OpenSearchController : Controller
	{
		private ISubjectRepository subjectRepository;
		public OpenSearchController(ISubjectRepository subjectRepository)
		{
			this.subjectRepository = subjectRepository;
		}

		//[HttpGet("suggest/{subjects_concat}/{search_term}", Name = "web-opensearch-suggest")]
		//public async Task<IEnumerable<Subject>> Suggest([FromRoute]string subjects_concat, [FromRoute]string search_term)
		//{
		//	var subjects = subjects_concat.Split('-');
		//	var valid_subjects = new[] { "artist", "person", "song" };

		//	if (subjects.FirstOrDefault() == null)
		//		subjects = valid_subjects;
		//	else if (subjects.FirstOrDefault() == "all")
		//		subjects = valid_subjects;
		//	else if (!subjects.Intersect(valid_subjects).Any())
		//		subjects = valid_subjects;
		//	else
		//		subjects = subjects.Intersect(valid_subjects).ToArray();

		//	var results = await subjectRepository.Suggest(subjects, search_term);
		//	return results.ToList();
		//}
	}
}