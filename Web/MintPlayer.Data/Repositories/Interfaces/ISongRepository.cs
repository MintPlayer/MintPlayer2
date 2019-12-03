using System.Threading.Tasks;
using System.Collections.Generic;
using MintPlayer.Data.Dtos;

namespace MintPlayer.Data.Repositories.Interfaces
{
    public interface ISongRepository
    {
        IEnumerable<Song> GetSongs(bool include_relations = false);
        Song GetSong(int id, bool include_relations = false);
        Task<Song> InsertSong(Song song);
        Task<Song> UpdateSong(Song song);
        Task DeleteSong(int song_id);
        Task SaveChangesAsync();
    }
}
