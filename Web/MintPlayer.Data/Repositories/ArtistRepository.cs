﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Helpers;
using MintPlayer.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintPlayer.Data.Repositories
{
    internal class ArtistRepository : IArtistRepository
    {
        private IHttpContextAccessor http_context;
        private MintPlayerContext mintplayer_context;
        private UserManager<Entities.User> user_manager;
        private ArtistHelper artist_helper;
        private Jobs.Interfaces.IElasticSearchJobRepository elasticSearchJobRepository;
        public ArtistRepository(IHttpContextAccessor http_context, MintPlayerContext mintplayer_context, UserManager<Entities.User> user_manager, ArtistHelper artist_helper, Jobs.Interfaces.IElasticSearchJobRepository elasticSearchJobRepository)
        {
            this.http_context = http_context;
            this.mintplayer_context = mintplayer_context;
            this.user_manager = user_manager;
            this.artist_helper = artist_helper;
            this.elasticSearchJobRepository = elasticSearchJobRepository;
        }

        public IEnumerable<Artist> GetArtists(bool include_relations = false)
        {
            if (include_relations)
            {
                var artists = mintplayer_context.Artists
                    .Include(artist => artist.Members)
                        .ThenInclude(ap => ap.Person)
                    .Include(artist => artist.Songs)
                        .ThenInclude(@as => @as.Song)
                    .Include(artist => artist.Media)
                        .ThenInclude(m => m.Type)
                    .Select(artist => ToDto(artist, true));
                return artists;
            }
            else
            {
                var artists = mintplayer_context.Artists
                    .Select(artist => ToDto(artist, false));
                return artists;
            }
        }

        public Artist GetArtist(int id, bool include_relations = false)
        {
            if (include_relations)
            {
                var artist = mintplayer_context.Artists
                    .Include(a => a.Members)
                        .ThenInclude(ap => ap.Person)
                    .Include(a => a.Songs)
                        .ThenInclude(@as => @as.Song)
                    .Include(a => a.Media)
                        .ThenInclude(m => m.Type)
                    .SingleOrDefault(a => a.Id == id);
                return ToDto(artist, true);
            }
            else
            {
                var artist = mintplayer_context.Artists
                    .SingleOrDefault(a => a.Id == id);
                return ToDto(artist, false);
            }
        }

        public async Task<Artist> InsertArtist(Artist artist)
        {
            // Convert to entity
            var entity_artist = ToEntity(artist, mintplayer_context);

            // Get current user
            var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
            entity_artist.UserInsert = user;
            entity_artist.InsertedAt = DateTime.Now;

            // Add to database
            mintplayer_context.Artists.Add(entity_artist);
            await mintplayer_context.SaveChangesAsync();

            // Index
            var new_artist = ToDto(entity_artist);
            //var index_status = await elastic_client.IndexDocumentAsync(new_artist);
            var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
            {
                Subject = new_artist,
                SubjectStatus = Enums.eSubjectAction.Added,
                JobStatus = Enums.eJobStatus.Queued
            });

            return new_artist;
        }

        public async Task<Artist> UpdateArtist(Artist artist)
        {
            // Find existing artist
            var artist_entity = mintplayer_context.Artists
                .Include(a => a.Members)
                    .ThenInclude(ap => ap.Person)
                .SingleOrDefault(a => a.Id == artist.Id);

            // Set new properties
            artist_entity.Name = artist.Name;
            artist_entity.YearStarted = artist.YearStarted;
            artist_entity.YearQuit = artist.YearQuit;
            artist_entity.UpdatedAt = DateTime.Now;

            IEnumerable<Entities.ArtistPerson> to_add, to_remove, to_update;
            artist_helper.CalculateUpdatedMembers(artist_entity, artist, mintplayer_context, out to_add, out to_update, out to_remove);
            foreach (var item in to_remove)
                mintplayer_context.Remove(item);
            foreach (var item in to_add)
                mintplayer_context.Add(item);
            foreach (var item in to_update)
                mintplayer_context.Update(item);

            // Get current user
            var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
            artist_entity.UserUpdate = user;

            // Index
            var updated_artist = ToDto(artist_entity);
            //await elastic_client.UpdateAsync<Artist>(updated_artist, u => u.Doc(updated_artist));
            var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
            {
                Subject = updated_artist,
                SubjectStatus = Enums.eSubjectAction.Updated,
                JobStatus = Enums.eJobStatus.Queued
            });

            return updated_artist;
        }

        public async Task DeleteArtist(int artist_id)
        {
            // Find existing artist
            var artist = mintplayer_context.Artists.Find(artist_id);

            // Get current user
            var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
            artist.UserDelete = user;
            artist.DeletedAt = DateTime.Now;

            // Index
            var deleted_artist = ToDto(artist);
            //await elastic_client.DeleteAsync<Artist>(deleted_artist);
            var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
            {
                Subject = deleted_artist,
                SubjectStatus = Enums.eSubjectAction.Deleted,
                JobStatus = Enums.eJobStatus.Queued
            });
        }

        public async Task SaveChangesAsync()
        {
            await mintplayer_context.SaveChangesAsync();
        }

        #region Conversion methods
        internal static Artist ToDto(Entities.Artist artist, bool include_relations = false)
        {
            if (artist == null) return null;
            if (include_relations)
            {
                return new Artist
                {
                    Id = artist.Id,
                    Name = artist.Name,
                    YearStarted = artist.YearStarted,
                    YearQuit = artist.YearQuit,

                    DateUpdate = artist.UpdatedAt ?? artist.InsertedAt,

                    PastMembers = artist.Members.Where(ap => !ap.Active).Select(ap => PersonRepository.ToDto(ap.Person)).ToList(),
                    CurrentMembers = artist.Members.Where(ap => ap.Active).Select(ap => PersonRepository.ToDto(ap.Person)).ToList(),
                    Songs = artist.Songs.Select(@as => SongRepository.ToDto(@as.Song)).ToList(),
                    Media = artist.Media.Select(medium => MediumRepository.ToDto(medium, true)).ToList()
                };
            }
            else
            {
                return new Artist
                {
                    Id = artist.Id,
                    Name = artist.Name,
                    YearStarted = artist.YearStarted,
                    YearQuit = artist.YearQuit,

                    DateUpdate = artist.UpdatedAt ?? artist.InsertedAt,
                };
            }
        }
        internal static Entities.Artist ToEntity(Artist artist, MintPlayerContext mintplayer_context)
        {
            if (artist == null) return null;
            var entity_artist = new Entities.Artist
            {
                Id = artist.Id,
                Name = artist.Name,
                YearStarted = artist.YearStarted,
                YearQuit = artist.YearQuit
            };

            #region Members
            var artist_person = artist.CurrentMembers.Select(person => {
                var entity_person = mintplayer_context.People.Find(person.Id);
                return new Entities.ArtistPerson(entity_artist, entity_person) { Active = true };
            }).Union(artist.PastMembers.Select(person => {
                var entity_person = mintplayer_context.People.Find(person.Id);
                return new Entities.ArtistPerson(entity_artist, entity_person) { Active = false };
            }));
            entity_artist.Members = artist_person.ToList();
            #endregion
            #region Media
            entity_artist.Media = artist.Media.Select(m => {
                var medium = MediumRepository.ToEntity(m, mintplayer_context);
                medium.Subject = entity_artist;
                return medium;
            }).ToList();
            #endregion

            return entity_artist;
        }
        #endregion
    }
}
