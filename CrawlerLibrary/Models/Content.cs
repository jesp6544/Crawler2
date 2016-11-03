using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerLibrary.Models
{
    public class Content
    {
        public Content()
        {
        }

        [Key]
        public int id { get; set; }

        [Microsoft.Build.Framework.Required]
        public string tag { get; set; }

        [Microsoft.Build.Framework.Required]
        [MaxLength(800)]
        public string text { get; set; }

        public int page_id { get; set; }

        [ForeignKey("page_id")]
        public Page page { get; set; }
    }
}