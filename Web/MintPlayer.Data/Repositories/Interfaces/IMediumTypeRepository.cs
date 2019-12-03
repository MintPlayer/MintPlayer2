using System.Threading.Tasks;
using System.Collections.Generic;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
	public interface IMediumTypeRepository
	{
		IEnumerable<MediumType> GetMediumTypes(bool include_relations = false);
		MediumType GetMediumType(int id, bool include_relations = false);
		Task<MediumType> InsertMediumType(MediumType mediumType);
		Task<MediumType> UpdateMediumType(MediumType mediumType);
		Task DeleteMediumType(int medium_type_id);
		Task SaveChangesAsync();
	}
}
