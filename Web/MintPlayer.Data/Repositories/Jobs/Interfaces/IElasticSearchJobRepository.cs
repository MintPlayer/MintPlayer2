using System.Threading.Tasks;
using MintPlayer.Data.Dtos.Jobs;

namespace MintPlayer.Data.Repositories.Jobs.Interfaces
{
    public interface IElasticSearchJobRepository
    {
        Task<ElasticSearchIndexJob> InsertElasticSearchIndexJob(ElasticSearchIndexJob job);
        Task<ElasticSearchIndexJob> UpdateElasticSearchIndexJob(ElasticSearchIndexJob job);
        Task<ElasticSearchIndexJob> PopElasticSearchIndexJob();
        Task SaveChangesAsync();
    }
}
