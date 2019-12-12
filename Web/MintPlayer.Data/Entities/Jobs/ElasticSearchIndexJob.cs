using MintPlayer.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MintPlayer.Data.Entities.Jobs
{
    internal class ElasticSearchIndexJob : Job
    {
        public Subject Subject { get; set; }
        public eSubjectAction SubjectStatus { get; set; }
    }
}
