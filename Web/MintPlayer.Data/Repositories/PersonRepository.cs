using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MintPlayer.Data.Repositories
{
	internal class PersonRepository : IPersonRepository
	{
		private IHttpContextAccessor http_context;
		private MintPlayerContext mintplayer_context;
		private UserManager<Entities.User> user_manager;
		private Jobs.Interfaces.IElasticSearchJobRepository elasticSearchJobRepository;
		public PersonRepository(IHttpContextAccessor http_context, MintPlayerContext mintplayer_context, UserManager<Entities.User> user_manager, Jobs.Interfaces.IElasticSearchJobRepository elasticSearchJobRepository)
		{
			this.http_context = http_context;
			this.mintplayer_context = mintplayer_context;
			this.user_manager = user_manager;
			this.elasticSearchJobRepository = elasticSearchJobRepository;
		}

		public IEnumerable<Person> GetPeople(bool include_relations = false)
		{
			if (include_relations)
			{
				var people = mintplayer_context.People
					.Include(person => person.Artists)
						.ThenInclude(ap => ap.Artist)
					.Include(person => person.Media)
						.ThenInclude(m => m.Type)
					.Select(person => ToDto(person, true));
				return people;
			}
			else
			{
				var people = mintplayer_context.People
					.Select(person => ToDto(person, false));
				return people;
			}
		}

		public Person GetPerson(int id, bool include_relations = false)
		{
			if (include_relations)
			{
				var person = mintplayer_context.People
					.Include(p => p.Artists)
						.ThenInclude(ap => ap.Artist)
					.Include(p => p.Media)
						.ThenInclude(m => m.Type)
					.SingleOrDefault(p => p.Id == id);
				return ToDto(person, true);
			}
			else
			{
				var person = mintplayer_context.People
					.SingleOrDefault(p => p.Id == id);
				return ToDto(person, false);
			}
		}

		public async Task<Person> InsertPerson(Person person)
		{
			// Convert to entity
			var entity_person = ToEntity(person, mintplayer_context);

			// Get current user
			var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
			entity_person.UserInsert = user;
			entity_person.InsertedAt = DateTime.Now;

			// Add to database
			mintplayer_context.People.Add(entity_person);
			await mintplayer_context.SaveChangesAsync();

			// Index
			var new_person = ToDto(entity_person);
			//var index_status = await elastic_client.IndexDocumentAsync(new_person);
			var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
			{
				Subject = new_person,
				SubjectStatus = Enums.eSubjectAction.Added,
				JobStatus = Enums.eJobStatus.Queued
			});

			return new_person;
		}

		public async Task<Person> UpdatePerson(Person person)
		{
			// Find existing person
			var entity_person = mintplayer_context.People.Find(person.Id);

			// Set new properties
			entity_person.FirstName = person.FirstName;
			entity_person.LastName = person.LastName;
			entity_person.Born = person.Born;
			entity_person.Died = person.Died;

			// Get current user
			var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
			entity_person.UserUpdate = user;
			entity_person.UpdatedAt = DateTime.Now;

			// Update in database
			mintplayer_context.Entry(entity_person).State = EntityState.Modified;

			// Index
			var updated_person = ToDto(entity_person);
			//await elastic_client.UpdateAsync<Person>(updated_person, u => u.Doc(updated_person));
			var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
			{
				Subject = updated_person,
				SubjectStatus = Enums.eSubjectAction.Updated,
				JobStatus = Enums.eJobStatus.Queued
			});

			return updated_person;
		}

		public async Task DeletePerson(int person_id)
		{
			// Find existing person
			var person = mintplayer_context.People.Find(person_id);

			// Get current user
			var user = await user_manager.GetUserAsync(http_context.HttpContext.User);
			person.UserDelete = user;
			person.DeletedAt = DateTime.Now;

			// Index
			var deleted_person = ToDto(person);
			//await elastic_client.DeleteAsync<Person>(deleted_person);
			var job = await elasticSearchJobRepository.InsertElasticSearchIndexJob(new Dtos.Jobs.ElasticSearchIndexJob
			{
				Subject = deleted_person,
				SubjectStatus = Enums.eSubjectAction.Deleted,
				JobStatus = Enums.eJobStatus.Queued
			});
		}

		public async Task SaveChangesAsync()
		{
			await mintplayer_context.SaveChangesAsync();
		}

		#region Conversion methods
		internal static Person ToDto(Entities.Person person, bool include_relations = false)
		{
			if (person == null) return null;
			if (include_relations)
			{
				return new Person
				{
					Id = person.Id,
					FirstName = person.FirstName,
					LastName = person.LastName,
					Born = person.Born,
					Died = person.Died,

					DateUpdate = person.UpdatedAt ?? person.InsertedAt,

					Artists = person.Artists.Select(ap => ArtistRepository.ToDto(ap.Artist)).ToList(),
					Media = person.Media.Select(medium => MediumRepository.ToDto(medium, true)).ToList()
				};
			}
			else
			{
				return new Person
				{
					Id = person.Id,
					FirstName = person.FirstName,
					LastName = person.LastName,
					Born = person.Born,
					Died = person.Died,

					DateUpdate = person.UpdatedAt ?? person.InsertedAt,
				};
			}
		}
		internal static Entities.Person ToEntity(Person person, MintPlayerContext mintplayer_context)
		{
			if (person == null) return null;
			var entity_person = new Entities.Person
			{
				Id = person.Id,
				FirstName = person.FirstName,
				LastName = person.LastName,
				Born = person.Born,
				Died = person.Died
			};
			entity_person.Media = person.Media.Select(m => {
				var medium = MediumRepository.ToEntity(m, mintplayer_context);
				medium.Subject = entity_person;
				return medium;
			}).ToList();
			return entity_person;
		}
		#endregion
	}
}
