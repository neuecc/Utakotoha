using System.Runtime.Serialization;

namespace Utakotoha.Model.Bing
{
    // <summary>Structure of Bing API Result</summary>
    [DataContract]
    public class SearchWebStructure
    {
        [DataMember]
        public Response SearchResponse { get; set; }

        [DataContract]
        public class Response
        {
            [DataMember]
            public Data Web { get; set; }

            [DataContract]
            public class Data
            {
                [DataMember]
                public SearchWebResult[] Results { get; set; }
            }
        }
    }

    [DataContract]
    public class SearchWebResult
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}