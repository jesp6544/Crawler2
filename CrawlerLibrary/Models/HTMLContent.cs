using SolrNet.Attributes;
using System.Collections.Generic;

namespace CrawlerLibrary.Models
{
    public class HTMLContent
    {
        [SolrField("absoluteuri")]
        public string AbsoluteUri { get; set; }

        [SolrUniqueKey("id")]
        public long DiscoveryID { get; set; }

        [SolrField("score")]
        public double Score { get; set; }

        [SolrField("title")]
        public string Title { get; set; }

        [SolrField("extension")]
        public string Extension { get; set; }

        [SolrField("publishdate")]
        public string PublishDate { get; set; }

        [SolrField("text")]
        public ICollection<string> Text { get; set; }
    }
}