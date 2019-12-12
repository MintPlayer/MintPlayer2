using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MintPlayer.Data.Repositories.Interfaces;
using MintPlayer.Web.ViewModels.Subject;
using Spa.SpaRoutes.CurrentSpaRoute;

namespace MintPlayer.Web.Server.Controllers.Web
{
    [ApiController]
    [Route("web/[controller]")]
    public class OpenSearchController : Controller
    {
        private readonly ISubjectRepository subjectRepository;
        private readonly ISpaRouteService spaRouteService;
        public OpenSearchController(ISubjectRepository subjectRepository, ISpaRouteService spaRouteService)
        {
            this.subjectRepository = subjectRepository;
            this.spaRouteService = spaRouteService;
        }

        [HttpGet("suggest/{search_term}", Name = "web-opensearch-suggest")]
        public async Task<IEnumerable<object>> Suggestions(string search_term)
        {
            var valid_subjects = new[] { "artist", "person", "song" };

            var results = await subjectRepository.Suggest(valid_subjects, search_term);
            return new object[] { search_term, results.Select(s => s.Text).ToArray() };
        }

        [HttpGet("redirect/{search_term}", Name = "web-opensearch-redirect")]
        public async Task<IActionResult> DoRedirect(string search_term)
        {
            var exact_matches = await subjectRepository.Search(new[] { "person", "artist", "song" }, search_term, true);
            if(exact_matches.Count == 1)
            {
                var subject_type = exact_matches.First().GetType();
                string subject_url;
                if(subject_type == typeof(Data.Dtos.Person))
                    subject_url = spaRouteService.GenerateUrl("person-show", new { id = exact_matches.First().Id });
                else if (subject_type == typeof(Data.Dtos.Artist))
                    subject_url = spaRouteService.GenerateUrl("artist-show", new { id = exact_matches.First().Id });
                else if (subject_type == typeof(Data.Dtos.Song))
                    subject_url = spaRouteService.GenerateUrl("song-show", new { id = exact_matches.First().Id });
                else
                    throw new Exception("The specified type is not a Subject");


                return Redirect(subject_url);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}