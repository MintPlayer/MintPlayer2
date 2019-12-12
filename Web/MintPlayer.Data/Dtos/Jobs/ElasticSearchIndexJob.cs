using MintPlayer.Data.Enums;

namespace MintPlayer.Data.Dtos.Jobs
{
    public class ElasticSearchIndexJob : Job
    {
        public Subject Subject { get; set; }
        public eSubjectAction SubjectStatus { get; set; }
    }
}
