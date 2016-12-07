using SolrNet.Attributes;

namespace CrawlerLibrary.Models
{
    public class Image
    {
        [SolrField("path")]
        public string Path { get; set; }

        [SolrField("altText")]
        public string AltText { get; set; }
    }
}