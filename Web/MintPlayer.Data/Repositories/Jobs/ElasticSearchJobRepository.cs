using MintPlayer.Data.Dtos.Jobs;
using MintPlayer.Data.Repositories.Jobs.Interfaces;
using System.Threading.Tasks;

namespace MintPlayer.Data.Repositories.Jobs
{
    internal class ElasticSearchJobRepository : IElasticSearchJobRepository
    {
        private MintPlayerContext mintplayer_context;
        public ElasticSearchJobRepository(MintPlayerContext mintplayer_context)
        {
            this.mintplayer_context = mintplayer_context;
        }

        public async Task<ElasticSearchIndexJob> InsertElasticSearchIndexJob(ElasticSearchIndexJob job)
        {
            // Convert to entity
            var entity_job = ToEntity(job, mintplayer_context);

            // Add to database
            mintplayer_context.ElasticSearchIndexJobs.Add(entity_job);
            await mintplayer_context.SaveChangesAsync();

            var new_job = ToDto(entity_job);
            return new_job;
        }

        #region Conversion methods
        internal static ElasticSearchIndexJob ToDto(Entities.Jobs.ElasticSearchIndexJob job, bool include_relations = false)
        {
            if (job == null) return null;
            return new ElasticSearchIndexJob
            {
                Id = job.Id,
                JobStatus = job.Status,
                Subject = SubjectRepository.ToDto(job.Subject, include_relations),
                SubjectStatus = job.SubjectStatus
            };
        }
        internal static Entities.Jobs.ElasticSearchIndexJob ToEntity(ElasticSearchIndexJob job, MintPlayerContext mintplayer_context)
        {
            if (job == null) return null;
            var entity_job = new Entities.Jobs.ElasticSearchIndexJob
            {
                Id = job.Id,
                Status = job.JobStatus,
                Subject = mintplayer_context.Subjects.Find(job.Subject.Id),
                SubjectStatus = job.SubjectStatus
            };
            return entity_job;
        }
        #endregion
    }
}
