using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Repositories.Interfaces;
using MintPlayer.Web.ViewModels.MediumType;

namespace MintPlayer.Web.Controllers.Web
{
	[Route("web/[controller]")]
	[ApiController]
	public class MediumTypeController : Controller
	{
		private IMediumTypeRepository mediumTypeRepository;
		public MediumTypeController(IMediumTypeRepository mediumTypeRepository)
		{
			this.mediumTypeRepository = mediumTypeRepository;
		}

		// GET: api/MediumType
		[HttpGet(Name = "web-mediumtype-list")]
		public IEnumerable<MediumType> Get([FromHeader]bool include_relations = false)
		{
			var medium_types = mediumTypeRepository.GetMediumTypes();
			return medium_types.ToList();
		}

		// GET: api/MediumType/5
		[HttpGet("{id}", Name = "web-mediumtype-get")]
		public MediumType Get(int id, [FromHeader]bool include_relations = false)
		{
			var medium_type = mediumTypeRepository.GetMediumType(id, include_relations);
			return medium_type;
		}

		// POST: api/MediumType
		[HttpPost(Name = "web-mediumtype-create")]
		[Authorize]
		public async Task<MediumType> Post([FromBody]MediumTypeCreateVM mediumTypeCreateVM)
		{
			var medium_type = await mediumTypeRepository.InsertMediumType(mediumTypeCreateVM.MediumType);
			return medium_type;
		}

		// PUT: api/MediumType/5
		[HttpPut("{id}", Name = "web-mediumtype-update")]
		[Authorize]
		public async Task Put(int id, [FromBody]MediumTypeUpdateVM mediumTypeUpdateVM)
		{
			await mediumTypeRepository.UpdateMediumType(mediumTypeUpdateVM.MediumType);
			await mediumTypeRepository.SaveChangesAsync();
		}

		// DELETE: api/MediumType/5
		[HttpDelete("{id}", Name = "web-mediumtype-delete")]
		[Authorize]
		public async Task Delete(int id)
		{
			await mediumTypeRepository.DeleteMediumType(id);
			await mediumTypeRepository.SaveChangesAsync();
		}
	}
}