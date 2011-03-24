using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Utakotoha.Model
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public string Url { get; private set; }

        public SearchResult(string title, string url)
        {
            this.Title = title;
            this.Url = url;
        }
    }
}