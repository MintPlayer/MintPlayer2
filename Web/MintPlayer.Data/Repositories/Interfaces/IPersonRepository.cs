using System.Collections.Generic;
using System.Threading.Tasks;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
	public interface IPersonRepository
	{
		IEnumerable<Person> GetPeople(bool include_relations = false);
		Person GetPerson(int id, bool include_relations = false);
		Task<Person> InsertPerson(Person person);
		Task<Person> UpdatePerson(Person person);
		Task DeletePerson(int person_id);
		Task SaveChangesAsync();
	}
}
