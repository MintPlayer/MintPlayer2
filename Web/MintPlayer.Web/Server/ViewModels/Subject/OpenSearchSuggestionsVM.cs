using System.Collections.Generic;

namespace MintPlayer.Web.ViewModels.Subject
{
    public class OpenSearchSuggestionsVM
    {
        public string SearchTerm { get; set; }
        public IEnumerable<string> Suggestions { get; set; }
    }
}
