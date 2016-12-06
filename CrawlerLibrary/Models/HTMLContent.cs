using SolrNet.Attributes;
using System;
using System.Collections.Generic;

namespace CrawlerLibrary.Models {

    public class HTMLContent {

        [SolrField("id")]
        public string ID { get; set; }

        [SolrField("url")]
        public string URL { get; set; }

        [SolrField("title")]
        public string Title { get; set; }

        [SolrField("p")]
        public ICollection<string> P { get; set; }

        [SolrField("h1")]
        public ICollection<string> H1 { get; set; }

        [SolrField("h2")]
        public ICollection<string> H2 { get; set; }

        [SolrField("h3")]
        public ICollection<string> H3 { get; set; }

        [SolrField("publishdate")]
        public string PublishDate { get; set; } = String.Format("{0:MM/dd/yyyy}", DateTime.Now);

        /*[SolrField("absoluteuri")]
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
        public ICollection<string> Text { get; set; }*/
    }
}