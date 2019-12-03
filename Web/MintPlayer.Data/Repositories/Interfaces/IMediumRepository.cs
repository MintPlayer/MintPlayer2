using System.Collections.Generic;
using System.Threading.Tasks;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
	public interface IMediumRepository
	{
		Task StoreMedia(Subject subject, IEnumerable<Medium> media);
	}
}
