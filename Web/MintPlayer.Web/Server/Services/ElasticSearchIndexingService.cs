using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MintPlayer.Data.Repositories.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MintPlayer.Web.Server.Services
{
    public class ElasticSearchIndexingService : IHostedService, IDisposable
    {
        private Timer timer;
        //private readonly IElasticSearchJobRepository elasticSearchJobRepository;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IElasticClient elasticClient;
        public ElasticSearchIndexingService(IServiceScopeFactory serviceScopeFactory, IElasticClient elasticClient)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.elasticClient = elasticClient;
        }

        public async void Dispose()
        {
            if (timer != null)
                await timer.DisposeAsync();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(FindAndRunIndexJob, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private async void FindAndRunIndexJob(object state)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var elasticSearchJobRepository = scope.ServiceProvider.GetRequiredService<IElasticSearchJobRepository>();
                var job = await elasticSearchJobRepository.PopElasticSearchIndexJob();
                if (job != null)
                {
                    try
                    {
                        var subject_type = job.Subject.GetType();
                        if (subject_type == typeof(Data.Dtos.Person))
                        {
                            var index_status = await elasticClient.IndexDocumentAsync((Data.Dtos.Person)job.Subject);
                            if (!index_status.IsValid)
                            {
                                throw new Exception($"Could not index person with id {job.Subject.Id}");
                            }
                        }
                        else if (subject_type == typeof(Data.Dtos.Artist))
                        {
                            var index_status = await elasticClient.IndexDocumentAsync((Data.Dtos.Artist)job.Subject);
                            if (!index_status.IsValid)
                            {
                                throw new Exception($"Could not index artist with id {job.Subject.Id}");
                            }
                        }
                        else if (subject_type == typeof(Data.Dtos.Song))
                        {
                            var index_status = await elasticClient.IndexDocumentAsync((Data.Dtos.Song)job.Subject);
                            if (!index_status.IsValid)
                            {
                                throw new Exception($"Could not index song with id {job.Subject.Id}");
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown subject type");
                        }

                        // Mark job as completed
                        job.JobStatus = Data.Enums.eJobStatus.Completed;
                        await elasticSearchJobRepository.UpdateElasticSearchIndexJob(job);
                        await elasticSearchJobRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        job.JobStatus = Data.Enums.eJobStatus.Error;
                        await elasticSearchJobRepository.UpdateElasticSearchIndexJob(job);
                        await elasticSearchJobRepository.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (timer != null)
                timer.Change(Timeout.Infinite, 0);
        }
    }
}
