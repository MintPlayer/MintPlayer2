using AspNetCoreOpenSearch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MintPlayer.Web.Services
{
    public class OpenSearchService : IOpenSearchService
    {
        public async Task<RedirectResult> PerformSearch(string searchTerms)
        {
            return new RedirectResult($"/{searchTerms}");
        }

        public async Task<IEnumerable<string>> ProvideSuggestions(string searchTerms)
        {
            return new[] {
                new string(searchTerms.Reverse().ToArray())
            };
        }
    }
}
