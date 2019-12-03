using System.Threading.Tasks;
using System.Collections.Generic;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
    public interface IArtistRepository
    {
        IEnumerable<Artist> GetArtists(bool include_relations = false);
        Artist GetArtist(int id, bool include_relations = false);
        Task<Artist> InsertArtist(Artist artist);
        Task<Artist> UpdateArtist(Artist artist);
        Task DeleteArtist(int artist_id);
        Task SaveChangesAsync();
    }
}
