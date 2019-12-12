using AspNetCoreOpenSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MintPlayer.Web.Services
{
    public class OpenSearchService : IOpenSearchService
    {
        public IEnumerable<string> ProvideSuggestions(string searchTerm)
        {
            return new string[]
            {
                searchTerm,
                new string(searchTerm.Reverse().ToArray())
            };
        }
    }
}
