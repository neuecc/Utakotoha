using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utakotoha
{
    public class SearchResult
    {
        public string Title { get; private set; }
        public string Url { get; private set; }

        public SearchResult(string title, string url)
        {
            this.Title = title;
            this.Url = url;
        }
    }
}