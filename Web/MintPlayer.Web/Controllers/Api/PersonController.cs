﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MintPlayer.Data.Dtos;
using MintPlayer.Web.ViewModels.Person;
using MintPlayer.Data.Repositories.Interfaces;

namespace MintPlayer.Web.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class PersonController : Controller
	{
		private IPersonRepository personRepository;
		private IMediumRepository mediumRepository;
		public PersonController(IPersonRepository personRepository, IMediumRepository mediumRepository)
		{
			this.personRepository = personRepository;
			this.mediumRepository = mediumRepository;
		}

		// GET: api/Person
		[HttpGet]
		public IEnumerable<Person> Get([FromHeader]bool include_relations = false)
		{
			var people = personRepository.GetPeople(include_relations);
			return people.ToList();
		}

		// GET: api/Person/5
		[HttpGet("{id}", Order = 1)]
		public Person Get(int id, [FromHeader]bool include_relations = false)
		{
			var person = personRepository.GetPerson(id, include_relations);
			return person;
		}

		// POST: api/Person
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<Person> Post([FromBody] PersonCreateVM personCreateVM)
		{
			var person = await personRepository.InsertPerson(personCreateVM.Person);
			await mediumRepository.StoreMedia(personCreateVM.Person, personCreateVM.Person.Media);
			return person;
		}

		// PUT: api/Person/5
		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task Put(int id, [FromBody] PersonUpdateVM personUpdateVM)
		{
			await personRepository.UpdatePerson(personUpdateVM.Person);
			await mediumRepository.StoreMedia(personUpdateVM.Person, personUpdateVM.Person.Media);
			await personRepository.SaveChangesAsync();
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task Delete(int id)
		{
			await personRepository.DeletePerson(id);
			await personRepository.SaveChangesAsync();
		}
	}
}