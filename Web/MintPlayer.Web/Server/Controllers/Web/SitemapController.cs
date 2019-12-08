using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MintPlayer.Data.Repositories.Interfaces;
using SitemapXml;
using SitemapXml.DependencyInjection.Interfaces;

namespace MintPlayer.Web.Server.Controllers.Web
{
    [Controller]
    [Route("[controller]")]
    public class SitemapController : Controller
    {
        private readonly ISitemapXml sitemapXml;
        private readonly IPersonRepository personRepository;
        private readonly IArtistRepository artistRepository;
        private readonly ISongRepository songRepository;
        public SitemapController(ISitemapXml sitemapXml, IPersonRepository personRepository, IArtistRepository artistRepository, ISongRepository songRepository)
        {
            this.sitemapXml = sitemapXml;
            this.personRepository = personRepository;
            this.artistRepository = artistRepository;
            this.songRepository = songRepository;
        }

        [Produces("application/xml")]
        [HttpGet(Name = "web-sitemap-index")]
        public SitemapIndex Index()
        {
            const int per_page = 100;

            var people = personRepository.GetPeople().ToList();
            var artists = artistRepository.GetArtists().ToList();
            var songs = songRepository.GetSongs().ToList();

            var person_urls = sitemapXml.GetSitemapIndex(people, per_page, (perPage, page) => Url.RouteUrl("web-sitemap-sitemap", new { subject = "person", count = perPage, page }, Request.Scheme));
            var artist_urls = sitemapXml.GetSitemapIndex(artists, per_page, (perPage, page) => Url.RouteUrl("web-sitemap-sitemap", new { subject = "artist", count = perPage, page }, Request.Scheme));
            var song_urls = sitemapXml.GetSitemapIndex(songs, per_page, (perPage, page) => Url.RouteUrl("web-sitemap-sitemap", new { subject = "song", count = perPage, page }, Request.Scheme));

            return new SitemapIndex(person_urls.Concat(artist_urls).Concat(song_urls));
        }

        [Produces("application/xml")]
        [HttpGet("{subject}/{count}/{page}", Name = "web-sitemap-sitemap")]
        public IActionResult Sitemap(string subject, int count, int page)
        {
            IEnumerable<Data.Dtos.Subject> subjects;
            switch (subject.ToLower())
            {
                case "person":
                    subjects = personRepository.GetPeople().Skip((page - 1) * count).Take(count);
                    break;
                case "artist":
                    subjects = artistRepository.GetArtists().Skip((page - 1) * count).Take(count);
                    break;
                case "song":
                    subjects = songRepository.GetSongs().Skip((page - 1) * count).Take(count);
                    break;
                default:
                    return NotFound();
            }

            return Ok(new UrlSet(subjects.Select(p => {
                var url = new Url
                {
                    Loc = $"{Request.Scheme}://{Request.Host}/{subject}/{p.Id}",
                    ChangeFreq = SitemapXml.Enums.ChangeFreq.Monthly,
                    LastMod = p.DateUpdate,
                };
                url.Links.Add(new Link
                {
                    Rel = "alternate",
                    HrefLang = "nl",
                    Href = $"{Request.Scheme}://{Request.Host}/{subject}/{p.Id}?lang=nl"
                });
                url.Links.Add(new Link
                {
                    Rel = "alternate",
                    HrefLang = "fr",
                    Href = $"{Request.Scheme}://{Request.Host}/{subject}/{p.Id}?lang=fr"
                });
                return url;
            })));
        }
    }
}